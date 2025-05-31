using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

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
                Sprite = Sprites.SiriusMissileBay.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "artifact", "SiriusMissileBayV2", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "artifact", "SiriusMissileBayV2", "description"]
                    )
                    .Localize,
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
            if (p.key == null || !validSiriusParts.Contains(p.key))
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
