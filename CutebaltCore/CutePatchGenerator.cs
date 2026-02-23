using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CutebaltCore;

internal abstract record class RegisterType 
{
    internal sealed record class Patchable(
        string ClassName,
        string ShortClassName,
        string Namespace,
        HashSet<PatchSymbol> Symbols,
        Dictionary<string, List<string>> Prefixes,
        Dictionary<string, List<string>> Postfixes,
        Dictionary<string, List<string>> Finalizers,
        Dictionary<string, List<string>> Transpilers
    ) 
        : RegisterType;
    internal sealed record class Registerable(string ClassName) : RegisterType;
    internal sealed record class ManualPatchable(string ClassName) : RegisterType;
}

internal sealed record class PatchSymbol(string TypeSym, string MethodSym, bool IsVirtual, int? Priority);

internal static class CutePatchGenerator
{
    public static void Generate(
        SourceProductionContext ctx,
        ImmutableArray<RegisterType?> syn
    )
    {
        if (syn.IsDefaultOrEmpty)
        {
            return;
        }

        string body = "";

        foreach (var t in syn)
        {
            switch (t) 
            {
                case RegisterType.Registerable reg:
                {
                    var className = QuoteWriter.AddExpression(() => reg.ClassName);

                    var bodyLine = QuoteWriter.AddStatement(sb =>
                    {
                        sb.AppendLine($"        {className}.Register(package, helper);");

                        return sb.ToString();
                    });

                    body += bodyLine;
                    break;
                }
            }
        }

        var registerableQuote = $$"""
            // Source Generated Code by CutebaltCore  :>>>>
            using Nanoray.PluginManager;
            using Nickel;

            namespace CutebaltCore;

            public static class Registerables
            {
                public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
                {
            {{body}}                
                } 
            }
            """;

        ctx.AddSource("CutebaltCore.Registeration.g.cs", registerableQuote);
    }
}