using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CutebaltCore;

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
        var sprites = new List<(string, string)>();
        foreach (var str in synString)
        {
            string dirName = Path.GetFileName(Path.GetDirectoryName(str));
            string filename = Path.GetFileNameWithoutExtension(str);

            string relativeUri = new Uri(projectDir).MakeRelativeUri(new Uri(str)).ToString();

            sprites.Add((dirName + "_" + filename, relativeUri));
        }

        var fieldBuilder = new StringBuilder();
        var assignmentBuilder = new StringBuilder();

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
