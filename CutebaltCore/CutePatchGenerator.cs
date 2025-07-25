using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CutebaltCore;

internal static class CutePatchGenerator
{
    public static void Generate(
        SourceProductionContext ctx,
        Compilation comp,
        ImmutableArray<TypeDeclarationSyntax?> syn
    )
    {
        if (syn.IsDefaultOrEmpty)
        {
            return;
        }

        string body = "";

        foreach (var symbol in GetSerializableSymbols(comp, syn, "IRegisterable"))
        {
            var className = QuoteWriter.AddExpression(() => symbol.ToFullDisplayString());

            var bodyLine = QuoteWriter.AddStatement(sb =>
            {
                sb.AppendLine($"        {className}.Register(package, helper);");

                return sb.ToString();
            });

            body += bodyLine;
        }

        string anotherBody = "";

        foreach (var symbol in GetSerializableSymbols(comp, syn, "IPatchable"))
        {
            StringBuilder builder = new StringBuilder();
            WritePatches(symbol, builder);
            anotherBody += QuoteWriter.AddStatement(sb =>
            {
                sb.AppendLine($"        {symbol.ToFullDisplayString()}.Patch(harmony);");
                return sb.ToString();
            });
            var ns = symbol.GetSymbolNamespace();
            ctx.AddSource(
                $"CutebaltCore.{symbol.Name}.g.cs",
                $$"""
                // Source Generated Code by CutebaltCore  :>>>>>>
                using HarmonyLib;
                using Nickel;

                namespace {{ns}};

                partial class {{symbol.Name}} 
                {
                    public static void Patch(IHarmony harmony) 
                    {
                {{builder}}
                    }
                }

                """
            );
        }

        string justAnotherBody = "";

        foreach (var symbol in GetSerializableSymbols(comp, syn, "IManualPatchable"))
        {
            var className = QuoteWriter.AddExpression(symbol.ToFullDisplayString);

            var bodyLine = QuoteWriter.AddStatement(sb =>
            {
                sb.AppendLine($"        {className}.ManualPatch(harmony);");

                return sb.ToString();
            });

            justAnotherBody += bodyLine;
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

    private static void WritePatches(INamedTypeSymbol symbol, StringBuilder builder)
    {
        HashSet<(string typeSym, string, bool, int?)> symbols =
            new HashSet<(string typeSym, string, bool, int?)>();

        Dictionary<string, List<string>> prefixBatches = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>> postfixBatches = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>> finalizerBatches = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>> transpilerBatches = new Dictionary<string, List<string>>();

        var members = symbol.GetMembers();
        foreach (var member in members)
        {
            if (member is IMethodSymbol method)
            {
                var name = method.ToDisplayString();
                int last = name.IndexOf('(');
                name = name.Substring(0, last);

                var attributes = method.GetAttributes();

                foreach (var attribute in attributes)
                {
                    var type = attribute.AttributeClass!.TypeArguments[0].ToFullDisplayString();
                    var methodName = attribute.ConstructorArguments[0];
                    var priority = attribute.ConstructorArguments[1];

                    symbols.Add(
                        (
                            type,
                            methodName.ToCSharpString(),
                            attribute.AttributeClass.Name.Contains("Virtual"),
                            int.Parse(priority.Value!.ToString())
                        )
                    );

                    switch (attribute.AttributeClass.Name)
                    {
                        case "OnPrefix":
                            AddToBatch(
                                prefixBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnPostfix":
                            AddToBatch(
                                postfixBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnTranspiler":
                            AddToBatch(
                                transpilerBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnFinalizer":
                            AddToBatch(
                                finalizerBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnVirtualPrefix":
                            AddToBatch(
                                prefixBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnVirtualPostfix":
                            AddToBatch(
                                postfixBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnVirtualTranspiler":
                            AddToBatch(
                                transpilerBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                        case "OnVirtualFinalizer":
                            AddToBatch(
                                finalizerBatches,
                                $"{type}+{methodName.ToCSharpString()}",
                                name
                            );
                            break;
                    }
                }
            }
        }

        foreach (var sym in symbols)
        {
            RETRY:
            string key = $"{sym.Item1}+{sym.Item2}";
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

            if (sym.Item4 is { } priority)
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
                        harmony.{(sym.Item3 ? "PatchVirtual" : "Patch")}(
                            AccessTools.DeclaredMethod(typeof({sym.Item1}), {sym.Item2}),
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

        void AddToBatch(Dictionary<string, List<string>> batch, string key, string methodName)
        {
            if (batch.TryGetValue(key, out var val))
            {
                val.Add(methodName);
                return;
            }

            var list = new List<string>() { methodName };
            batch.Add(key, list);
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetSerializableSymbols(
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax?> syn,
        string serialize
    )
    {
        foreach (var partialClass in syn)
        {
            if (partialClass is null)
            {
                continue;
            }
            var model = compilation.GetSemanticModel(partialClass.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(partialClass);
            if (symbol is null)
            {
                continue;
            }

            if (HasInterfaces(symbol, serialize))
            {
                yield return symbol;
            }
        }
    }

    private static bool HasInterfaces(INamedTypeSymbol symbol, string interfaceName)
    {
        return symbol.Interfaces.Any(intfa => intfa.Name.StartsWith(interfaceName));
    }
}