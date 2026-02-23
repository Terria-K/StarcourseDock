using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CutebaltCore;

internal static class CuteImagePathGenerator
{
    public static void Generate(
        SourceProductionContext ctx,
        ImmutableArray<string> synString,
        string? projectDir
    )
    {
        if (projectDir is null)
        {
            return;
        }

        var fieldBuilder = new StringBuilder();
        var assignmentBuilder = new StringBuilder();

        foreach (var str in synString)
        {
            string dirName = Path.GetFileName(Path.GetDirectoryName(str));
            string filename = Path.GetFileNameWithoutExtension(str);

            string relativeUri = new Uri(projectDir).MakeRelativeUri(new Uri(str)).ToString();

            string fileID = dirName + "_" + filename;
            string relPath = relativeUri;

            fieldBuilder.AppendLine($"public static ISpriteEntry {fileID} = null!;");
            assignmentBuilder.AppendLine($"""
            {fileID} = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("{relPath}")
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
