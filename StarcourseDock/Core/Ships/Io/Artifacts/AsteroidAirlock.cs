using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AsteroidAirlock : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "AsteroidAirlock",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_AsteroidAirlock.Sprite,
                Name = Localization.ship_Io_artifact_AsteroidAirlock_name(),
                Description = Localization.ship_Io_artifact_AsteroidAirlock_description(),
            }
        );
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTGlossary("midrow.asteroid"),
            new TTCard() { card = new PulledAndRelease() }
        ];
    }
}

