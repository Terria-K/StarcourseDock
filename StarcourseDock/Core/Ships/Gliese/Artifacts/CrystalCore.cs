using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CrystalCore : Artifact, IRegisterable
{
    public List<Part>? tempParts;
    internal static ICardTraitEntry FrozenTrait { get; set; } = null!;

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

        FrozenTrait = helper.Content.Cards.RegisterTrait(
            "Frozen",
            new()
            {
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "trait", "Frozen", "name"])
                    .Localize,
                Icon = (State s, Card? c) => Sprites.icons_frozen.Sprite,
            }
        );
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

        card.unplayableOverride = true;
        ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
            state,
            card,
            FrozenTrait,
            true,
            false
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
                    type = PType.special,
                    skin = GlieseShip.GlieseCrystal2.UniqueName,
                    stunModifier = PStunMod.breakable,
                    key = "crystal2::StarcourseDock",
                }
            );
        }
    }
}
