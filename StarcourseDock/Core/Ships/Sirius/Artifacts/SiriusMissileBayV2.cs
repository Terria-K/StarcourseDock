using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusMissileBayV2 : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SiriusMissileBayV2",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_SiriusMissileBayV2.Sprite,
                Name = Localization.ship_Sirius_artifact_SiriusMissileBayV2_name(),
                Description = Localization.ship_Sirius_artifact_SiriusMissileBayV2_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new SiriusMissileBay().Key() });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        HashSet<string> validSiriusParts = ["firstBay", "weak"];
        int activeBayCount = 0;

        foreach (Part p in state.ship.parts)
        {
            if (p.key == null || !validSiriusParts.Contains(p.key) || !p.active)
            {
                continue;
            }

            activeBayCount += 1;
        }

        if (activeBayCount > 1)
        {
            combat.Queue(new AToggleMissileBay());
        }

        if (!combat.HasCardOnHand<BarrageMode>())
        {
            combat.Queue(
                new AAddCard { card = new BarrageMode(), destination = CardDestination.Hand }
            );
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard() { card = new BarrageMode() }];
    }
}
