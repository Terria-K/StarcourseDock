using Nanoray.PluginManager;
using Nickel;

namespace Teuria.AttackDrone;

internal sealed class DroneMkI : Artifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DroneMkI",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifacts", "DroneMkI", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifacts", "DroneMkI", "description"]).Localize,
                ArtifactType = typeof(DroneMkI),
                Sprite = helper.Content.Sprites.RegisterSprite(
                    "DroneMkI",
                    package.PackageRoot.GetRelativeFile("assets/DroneMkI.png")
                ).Sprite,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true
                }
            }
        );
    }
}
