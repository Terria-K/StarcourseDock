using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GravitationalPull : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "GravitationalPull",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_GravitationalPull.Sprite,
                Name = Localization.ship_Io_artifact_GravitationalPull_name(),
                Description = Localization.ship_Io_artifact_GravitationalPull_description(),
            }
        );
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard() { card = new PulledAndRelease() }
        ];
    }
}
