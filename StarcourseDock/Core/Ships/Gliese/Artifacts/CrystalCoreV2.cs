using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CrystalCoreV2 : Artifact
{
    public List<Part>? tempParts;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "CrystalCoreV2",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_CrystalCoreV2.Sprite,
                Name = Localization.ship_Gliese_artifact_CrystalCoreV2_name(),
                Description = Localization.ship_Gliese_artifact_CrystalCoreV2_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new CrystalCore().Key() });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
            combat.Queue(
                new AAddCard() { card = new AbsoluteFreeze(), destination = CardDestination.Deck }
            );
            return;
        }

        combat.Queue(new ARemoveAllBrokenPart());
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        combat.QueueImmediate(new ATempToBreakable());
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.QueueImmediate(new ARepairAllBrokenPart());
        state.rewardsQueue.QueueImmediate(new AResetShip { parts = tempParts });
    }

    public override void OnPlayerPlayCard(
        int energyCost,
        Deck deck,
        Card card,
        State state,
        Combat combat,
        int handPosition,
        int handCount
    )
    {
        combat.QueueImmediate(
            new AFreezeCard()
            {
                selectedCard = card,
                increment = 1,
                mustHaveTheTrait = true,
            }
        );
    }

    public override void OnPlayerTakeNormalDamage(
        State state,
        Combat combat,
        int rawAmount,
        Part? part
    )
    {
        if (part != null && part.key == "portal")
        {
            state.ship.InsertPart(
                state,
                "portal",
                "portal",
                false,
                new Part()
                {
                    type = PType.cannon,
                    skin = GlieseShip.GlieseCannonTemp.UniqueName,
                    stunModifier = PStunMod.breakable,
                    key = "crystal_tempcannon::StarcourseDock",
                }
            );
            Pulse();
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard() { card = new AbsoluteFreeze() }];
    }
}
