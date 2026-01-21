using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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


        var localProvider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is TypeDeclarationSyntax syntax && syntax.BaseList?.Types.Count > 0,
                static (ctx, _) =>
                {
                    var syntax = (TypeDeclarationSyntax)ctx.Node;
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(syntax);
                    if (symbol is null)
                    {
                        return null;
                    }
                    var interfaces = symbol.Interfaces;

                    foreach (var interf in interfaces)
                    {
                        string fullname = interf.ToDisplayString();
                        if (fullname == "CutebaltCore.ILocalProvider" || fullname == "Nickel.ILocalizationProvider<string>")
                        {
                            return new LocalType(symbol.ToFullDisplayString(), symbol.Name, fullname == "Nickel.ILocalizationProvider<string>");
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

        var projectDir = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var dir);
                return dir;
            });

        var pngFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".png"));
        var pngFilesPath = pngFiles.Select((x, token) => x.Path);

        var pngCompilation = context.CompilationProvider.Combine(pngFilesPath.Collect());
        var projectPngCompilation = pngCompilation.Combine(projectDir);

        context.RegisterSourceOutput(
            projectPngCompilation,
            static (ctx, source) => CuteImagePathGenerator.Generate(ctx, source.Left.Left, source.Left.Right, source.Right)
        );
    }
}

internal static class CuteImagePathGenerator
{
    public static void Generate(
        SourceProductionContext ctx,
        Compilation comp,
        ImmutableArray<string> synString,
        string? projectDir
    )
    {
        if (projectDir is null)
        {
            return;
        }
        var syn = comp.SyntaxTrees.First(x => x.HasCompilationUnitRoot);
        var dir = syn.FilePath;
        List<(string, string)> sprites = new List<(string, string)>();
        foreach (var str in synString)
        {
            string dirName = Path.GetFileName(Path.GetDirectoryName(str));
            string filename = Path.GetFileNameWithoutExtension(str);

            string relativeUri = new Uri(projectDir).MakeRelativeUri(new Uri(str)).ToString();

            sprites.Add((dirName + "_" + filename, relativeUri));
        }

        StringBuilder fieldBuilder = new StringBuilder();
        StringBuilder assignmentBuilder = new StringBuilder();

        foreach (var sprite in sprites)
        {
            fieldBuilder.AppendLine($"public static ISpriteEntry {sprite.Item1} = null!;");
            assignmentBuilder.AppendLine($"""
            {sprite.Item1} = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("{sprite.Item2}")
            );
            """);
        }

        ctx.AddSource(
            "Sprites.g.cs",
            $$"""
            using Nanoray.PluginManager;
            using Nickel;

            namespace Teuria.StarcourseDock;

            internal static class Sprites 
            {
            {{fieldBuilder}}

                public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
                {
            {{assignmentBuilder}}
                }
            }
            """
        );
    }
}
