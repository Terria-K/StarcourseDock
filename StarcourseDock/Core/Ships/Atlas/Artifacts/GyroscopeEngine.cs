using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GyroscopeEngine : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "GyroscopeEngine",
            new()
            {
                Name = Localization.ship_Atlas_artifact_GyroscopeEngine_name(),
                Description = Localization.ship_Atlas_artifact_GyroscopeEngine_description(),
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true
                },
                Sprite = Sprites.artifacts_GyroscopeEngine.Sprite
            }
        );
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        double center = (handCount - 1) / 2.0;
        double distance = handPosition - center;

        int dir = (int)Math.Round(distance, MidpointRounding.AwayFromZero);

        if ((handCount & 1) == 0 && dir == 0)
        {
            dir = (distance > 0) ? 1 : -1;
        }
        else if (dir == 0)
        {
            return;
        }

        combat.Queue(new AMove()
        {
            targetPlayer = true,
            dir = dir
        });
    }
}