using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CrystalCore : Artifact, IRegisterable
{
    public List<Part>? tempParts;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "CrystalCore",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_CrystalCore.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "artifact", "CrystalCore", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "artifact", "CrystalCore", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [GlieseKit.GetFrozenTraitTooltip()];
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
            return;
        }

        combat.Queue(new ARemoveAllBrokenPart());
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
        if (card is Unfreeze)
        {
            return;
        }

        if (
            !ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(
                state,
                card,
                GlieseKit.FrozenTrait
            )
        )
        {
            ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", 0);
        }

        var frozenCount = ModEntry.Instance.Helper.ModData.GetModDataOrDefault<int>(
            card,
            "FrozenCount"
        );

        if (frozenCount >= 3)
        {
            card.unplayableOverride = true;
            ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", frozenCount + 1);
            return;
        }

        ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
            state,
            card,
            GlieseKit.FrozenTrait,
            true,
            false
        );

        ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", frozenCount + 1);
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
                    type = PType.special,
                    skin = "crystal_2",
                    stunModifier = PStunMod.breakable,
                    key = "crystal2::StarcourseDock",
                }
            );
        }
    }
}
