using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using YamlDotNet.Serialization;

namespace CutebaltCore;

internal static class CuteLocalGenerator
{
    private static HashSet<(string key, string? value)> keys = new HashSet<(string key, string? value)>();

    public static void Generate(
        SourceProductionContext ctx,
        Compilation comp,
        ImmutableArray<TypeDeclarationSyntax?> syn,
        ImmutableArray<string> synString
    )
    {
        Regex regex = new Regex("{{([ \\w\\.\\-_]+)}}");
        if (syn.IsDefaultOrEmpty)
        {
            return;
        }

        HashSet<string> availableLanguages = ["en"];
        Dictionary<string, List<(string, string, Func<string, string>?)>> membersAndValue = [];
        membersAndValue.Add("en", []);

        ulong typeId = 0;
        StringBuilder types = new StringBuilder();
        StringBuilder methods = new StringBuilder();
        StringBuilder locals = new StringBuilder();

        foreach (var text in synString)
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            var yaml = deserializer.Deserialize(text);

            if (yaml is Dictionary<object, object> dict)
            {
                foreach (var d in dict)
                {
                    string key = d.Key.ToString();
                    keys.Add((key, null));
                    LookupForKeys(key, d.Value);
                }
            }

            void LookupForKeys(string prefix, object? value)
            {
                if (value is Dictionary<object, object> obj)
                {
                    foreach (var d in obj)
                    {
                        if (d.Value is Dictionary<object, object> vObj)
                        {
                            string key = d.Key.ToString();
                            keys.Add((prefix + "." + key, null));

                            LookupForKeys(prefix + "." + key, vObj);
                        }

                        if (d.Value is string str)
                        {
                            string key = d.Key.ToString();
                            keys.Add((prefix + "." + key, str));
                        }

                        if (d.Value is List<object> arr)
                        {
                            StringBuilder builderArr = new StringBuilder();
                            string key = d.Key.ToString();

                            foreach (var s in arr)
                            {
                                builderArr.AppendLine(s.ToString());
                            }

                            keys.Add((prefix + "." + key, builderArr.ToString()));
                        }
                    }
                }
            }

            StringBuilder fieldBuilder = new StringBuilder();
            foreach (var allKey in keys)
            {
                string lang = "en";
                string name = allKey.key;
                string? value = allKey.value;
                if (value is null)
                {
                    continue;
                }

                Func<string, string>? method = null;
                string token = "null";
                string functionParameters = "";
                var match = regex.Match(value);
                if (match is not null)
                {
                    StringBuilder replacedValues = new StringBuilder();
                    List<string> matchedValues = new List<string>();
                    List<string> functionParamatersValues = new List<string>();

                    for (int i = 1; i <= match.Groups.Count; i += 1)
                    {
                        string v = match.Groups[i].Value;
                        if (!string.IsNullOrEmpty(v))
                        {
                            matchedValues.Add(v);
                            replacedValues.AppendLine($".Replace(\"{{{{{v}}}}}\", token.{v})");
                            functionParamatersValues.Add($"string {v}");
                        }
                    }

                    if (matchedValues.Count != 0)
                    {
                        string parameters = string.Join(",", matchedValues);
                        functionParameters = string.Join(",", functionParamatersValues);
                        token = $"new RegexTypeGenerated{typeId}({parameters})";
                        types.AppendLine($$"""
                        record struct RegexTypeGenerated{{typeId}}({{functionParameters}});
                        """);

                        ulong y = typeId;

                        method = (x) => $""""
                        GeneratedLocal{y}(
                        """
                        {x}
                        """, (RegexTypeGenerated{y})tokens)
                        """";

                        methods.AppendLine($$$"""
                        private static string GeneratedLocal{{{typeId}}}(string localizeStr, RegexTypeGenerated{{{typeId}}} token) 
                        {
                            return localizeStr
                                {{{replacedValues}}}
                                ;
                        }
                        """);

                        typeId += 1;
                    }
                }

                fieldBuilder.AppendLine($"public static SingleLocalizationProvider {name.Replace('.', '_')}({functionParameters}) => ModEntry.Instance.CsharpAnyLocalizations.Bind(\"{name}\", {token}).Localize;");
                fieldBuilder.AppendLine($"public static string Str_{name.Replace('.', '_')}({functionParameters}) => ModEntry.Instance.CsharpLocalizations.Localize(\"{name}\", {token});");
                if (membersAndValue.TryGetValue(lang, out var list))
                {
                    list.Add((name, value, method));
                    continue;
                }

                List<(string, string, Func<string, string>?)> l = [(name, value, method)];
                membersAndValue.Add(lang, l);
            }

            locals.AppendLine(
                $$"""
                partial class Localization
                {
                {{fieldBuilder}}
                }
                """
            );
        }

        StringBuilder builder = new StringBuilder();

        foreach (var symbol in GetSerializableSymbols(comp, syn, "ILocalizationProvider"))
        {
            var className = QuoteWriter.AddExpression(() => symbol.ToFullDisplayString());

            var bodyLine = QuoteWriter.AddStatement(sb =>
            {
                sb.AppendLine($"        {className}.Register(package, helper);");

                return sb.ToString();
            });

            foreach (var lang in availableLanguages)
            {
                StringBuilder fields = new StringBuilder();
                if (membersAndValue.TryGetValue(lang, out var list))
                {
                    foreach (var l in list)
                    {
                        if (l.Item3 != null)
                        {
                            fields.AppendLine($"""
                                                "{l.Item1}" => 
                                {l.Item3(l.Item2)},
                                """
                            );
                        }
                        else
                        {
                            fields.AppendLine($""""
                                                "{l.Item1}" => 
                                """
                                {l.Item2}
                                """,
                                """"
                            );
                        }
                    }
                }
                builder.AppendLine($$"""
                        if (locale == "{{lang}}") 
                        {
                            return key switch 
                            {
                {{fields}}
                                _ => null 
                            };
                        }
                """);
            }

            ctx.AddSource(
                $"CutebaltCore.{symbol.Name}.g.cs",
                $$"""
                #nullable enable
                using Nickel;
                namespace Teuria.StarcourseDock;

                partial class {{symbol.Name}} : ILocalizationProvider<string>
                {
                    public string? Localize(string locale, string key, object? tokens = null)
                    {
                {{builder}}

                        return null;
                    }

                    {{methods}}
                }
                {{locals}}

                {{types}}
                """
            );
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
