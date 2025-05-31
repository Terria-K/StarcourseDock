using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class DoubleStar : Artifact
{
    public bool binaryStarDetected = false;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DoubleStar",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_DoubleStar.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Albireo", "artifact", "DoubleStar", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Albireo", "artifact", "DoubleStar", "description"]
                    )
                    .Localize,
            }
        );

        var evadeAction = ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterAction(
            new DoubleStarAction(),
            10
        );
        evadeAction.RegisterPaymentOption(new DoubleStarPaymentOption());
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard { card = new RelativeMotion() }];
    }

    public override Spr GetSprite()
    {
        if (binaryStarDetected)
        {
            return Sprites.artifacts_DoubleStarInactive.Sprite;
        }
        return Sprites.artifacts_DoubleStar.Sprite;
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(
            new AAddCard() { card = new RelativeMotion(), destination = CardDestination.Hand }
        );
        combat.Queue(new ACheckParts());
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        binaryStarDetected = combat.modifier is MBinaryStar;
        if (state.ship.x > 11)
        {
            combat.QueueImmediate(new AEnergy() { changeAmount = -1 });
        }
        else if (state.ship.x < 3)
        {
            combat.QueueImmediate(new AEnergy() { changeAmount = 1 });
        }
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
        if (binaryStarDetected)
        {
            return;
        }

        combat.Queue(new ACheckParts());
        if (state.ship.x > 11)
        {
            Pulse();
            combat.Queue(new ADoubler() { card = card });
        }
    }

    public override bool? ModifyAttacksToStun(State state, Combat? combat)
    {
        if (binaryStarDetected)
        {
            return false;
        }
        return state.ship.x < 3;
    }
}
