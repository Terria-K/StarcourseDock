using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        string anotherBody = "";
        string justAnotherBody = "";

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
                case RegisterType.Patchable patchable:
                {
                    var builder = new StringBuilder();
                    WritePatches(patchable, builder);
                    anotherBody += QuoteWriter.AddStatement(sb =>
                    {
                        sb.AppendLine($"        {patchable.ClassName}.Patch(harmony);");
                        return sb.ToString();
                    });

                    ctx.AddSource(
                        $"CutebaltCore.{patchable.ShortClassName}.g.cs",
                        $$"""
                        // Source Generated Code by CutebaltCore  :>>>>>>
                        using HarmonyLib;
                        using Nickel;

                        namespace {{patchable.Namespace}};

                        partial class {{patchable.ShortClassName}} 
                        {
                            public static void Patch(IHarmony harmony) 
                            {
                        {{builder}}
                            }
                        }

                        """
                    );
                    break;
                }
                case RegisterType.ManualPatchable manualPatchable:
                {
                    var className = QuoteWriter.AddExpression(() => manualPatchable.ClassName);

                    var bodyLine = QuoteWriter.AddStatement(sb =>
                    {
                        sb.AppendLine($"        {className}.ManualPatch(harmony);");

                        return sb.ToString();
                    });

                    justAnotherBody += bodyLine;
                    break;
                }
            }
        }

        var reversePatchableQuotes = $$"""
            // Source Generated Code by CutebaltCore  :>>>>
            using Nickel;

            namespace CutebaltCore;

            public static class ManualPatchables
            {
                public static void Patch(IHarmony harmony)
                {
            {{justAnotherBody}}                
                } 
            }
            """;

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

        ctx.AddSource("CutebaltCore.ReversePatchables.g.cs", reversePatchableQuotes);
        ctx.AddSource("CutebaltCore.Registeration.g.cs", registerableQuote);
        ctx.AddSource(
            "CutebaltCore.Patchables.g.cs",
            $$"""
                 // Source Generated Code by CutebaltCore  :>>>>
                using Nickel;

                namespace CutebaltCore;

                public static class Patchables
                {
                    public static void Patch(IHarmony harmony)
                    {
                {{anotherBody}}
                    }
                }

            """
        );
    }

    private static void WritePatches(RegisterType.Patchable patchable, StringBuilder builder)
    {
        var symbols = patchable.Symbols;
        var prefixBatches = patchable.Prefixes;
        var postfixBatches = patchable.Postfixes;
        var finalizerBatches = patchable.Finalizers;
        var transpilerBatches = patchable.Transpilers;

        foreach (var sym in symbols)
        {
            RETRY:
            string key = $"{sym.TypeSym}+{sym.MethodSym}";
            int maxCount = 0;
            if (prefixBatches.TryGetValue(key, out var pb))
            {
                maxCount = pb.Count;
            }

            if (postfixBatches.TryGetValue(key, out var pob))
            {
                maxCount = Math.Max(maxCount, pob.Count);
            }

            if (transpilerBatches.TryGetValue(key, out var tb))
            {
                maxCount = Math.Max(maxCount, tb.Count);
            }

            if (finalizerBatches.TryGetValue(key, out var fb))
            {
                maxCount = Math.Max(maxCount, fb.Count);
            }

            bool needRetry = false;

            string? prefix = null;
            string? postfix = null;
            string? transpiler = null;
            string? finalizer = null;

            string secondArgs = "";

            if (sym.Priority is { } priority)
            {
                secondArgs = ", " + priority;
            }

            if (prefixBatches.TryGetValue(key, out var prefixList) && prefixList.Count > 0)
            {
                var first = prefixList[0];
                prefixList.RemoveAt(0);
                prefix = $"prefix: new HarmonyMethod({first}{secondArgs})";

                needRetry |= prefixList.Count > 0;
            }

            if (postfixBatches.TryGetValue(key, out var postfixList) && postfixList.Count > 0)
            {
                var first = postfixList[0];
                postfixList.RemoveAt(0);
                postfix = $"postfix: new HarmonyMethod({first}{secondArgs})";

                needRetry |= postfixList.Count > 0;
            }

            if (
                transpilerBatches.TryGetValue(key, out var transpilerList)
                && transpilerList.Count > 0
            )
            {
                var first = transpilerList[0];
                transpilerList.RemoveAt(0);
                transpiler = $"transpiler: new HarmonyMethod({first}{secondArgs})";

                needRetry |= transpilerList.Count > 0;
            }

            if (finalizerBatches.TryGetValue(key, out var finalizerList) && finalizerList.Count > 0)
            {
                var first = finalizerList[0];
                finalizerList.RemoveAt(0);
                finalizer = $"finalizer: new HarmonyMethod({first}{secondArgs})";

                needRetry |= finalizerList.Count > 0;
            }

            string harmonyPatch = $"""
                        harmony.{(sym.IsVirtual ? "PatchVirtual" : "Patch")}(
                            AccessTools.DeclaredMethod(typeof({sym.TypeSym}), {sym.MethodSym}),
                        /* prefix */    {TryAddTrailingCommas(
                    prefix,
                    postfix,
                    transpiler,
                    finalizer
                )}
                        /* postfix */   {TryAddTrailingCommas(postfix, transpiler, finalizer)}
                        /* transpiler */   {TryAddTrailingCommas(transpiler, finalizer)}
                        /* finalizer */   {finalizer}
                        );
                """;

            builder.AppendLine(harmonyPatch);

            if (needRetry)
            {
                goto RETRY;
            }
        }

        static string? TryAddTrailingCommas(string? target, params string?[] checks)
        {
            if (target is null)
            {
                return null;
            }

            for (int i = 0; i < checks.Length; i++)
            {
                if (checks[i] != null)
                {
                    return target + ",";
                }
            }
            return target;
        }
    }
}