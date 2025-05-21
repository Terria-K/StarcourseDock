using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class TinyWormhole : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("TinyWormhole", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.Common]
            },
            Sprite = Sprites.TinyWormhole.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "TinyWormhole", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "TinyWormhole", "description"]).Localize
        });
    }
}