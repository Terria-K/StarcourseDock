using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using YamlDotNet.Serialization;

namespace CutebaltCore;

[Generator]
internal class CuteGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                "IRegisterable.g.cs",
                SourceText.From(SourceGenerationHelper.IRegisterable, Encoding.UTF8)
            );

            ctx.AddSource(
                "IPatchable.g.cs",
                SourceText.From(SourceGenerationHelper.IPatchable, Encoding.UTF8)
            );

            ctx.AddSource(
                "IReversePatchable.g.cs",
                SourceText.From(SourceGenerationHelper.IReversePatchable, Encoding.UTF8)
            );

            ctx.AddSource(
                "ILocalProvider.g.cs",
                SourceText.From(SourceGenerationHelper.ILocalProvider, Encoding.UTF8)
            );

            ctx.AddSource(
                "Attributes.g.cs",
                SourceText.From(SourceGenerationHelper.Attributes, Encoding.UTF8)
            );
        });

        var patchProvider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is TypeDeclarationSyntax syntax && syntax.BaseList?.Types.Count > 0,
                static (ctx, _) =>
                {
                    var syntax = (TypeDeclarationSyntax)ctx.Node;
                    var interfaces = ctx.SemanticModel.GetDeclaredSymbol(syntax)?.Interfaces;

                    if (interfaces is null)
                    {
                        return null;
                    }

                    foreach (var interf in interfaces)
                    {
                        string fullname = interf.ToDisplayString();
                        if (
                            fullname == "CutebaltCore.IRegisterable"
                            || fullname == "CutebaltCore.IPatchable"
                        )
                        {
                            return syntax;
                        }
                    }

                    return null;
                }
            )
            .Where(x => x is not null);

        var patchCompilation = context.CompilationProvider.Combine(patchProvider.Collect());

        context.RegisterImplementationSourceOutput(
            patchCompilation,
            static (ctx, source) => CutePatchGenerator.Generate(ctx, source.Left, source.Right)
        );

        var ymlFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".yaml"));

        var ymlFileContents = ymlFiles.Select((text, token) => text.GetText(token)!.ToString());

        var ymlCompilation = context.CompilationProvider.Combine(ymlFileContents.Collect());

        // context.RegisterSourceOutput(
        //     ymlCompilation,
        //     static (ctx, source) => CuteYmlGenerator.Generate(ctx, source.Left, source.Right)
        // );

        var localProvider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is TypeDeclarationSyntax syntax && syntax.BaseList?.Types.Count > 0,
                static (ctx, _) =>
                {
                    var syntax = (TypeDeclarationSyntax)ctx.Node;
                    var interfaces = ctx.SemanticModel.GetDeclaredSymbol(syntax)?.Interfaces;

                    if (interfaces is null)
                    {
                        return null;
                    }

                    foreach (var interf in interfaces)
                    {
                        string fullname = interf.ToDisplayString();
                        if (fullname == "CutebaltCore.ILocalProvider" || fullname == "Nickel.ILocalizationProvider<string>")
                        {
                            return syntax;
                        }
                    }

                    return null;
                }
            )
            .Where(x => x is not null);

        var localCompilation = context.CompilationProvider.Combine(localProvider.Collect());
        var localAndYmlCompilation = localCompilation.Combine(ymlCompilation);

        context.RegisterSourceOutput(
            localAndYmlCompilation,
            static (ctx, source) => CuteLocalGenerator.Generate(ctx, source.Left.Left, source.Left.Right, source.Right.Right)
        );
    }
}
