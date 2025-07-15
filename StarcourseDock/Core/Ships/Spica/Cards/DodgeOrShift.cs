using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class DodgeOrShift : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(
            MethodBase.GetCurrentMethod()!.DeclaringType!.Name,
            new()
            {
                CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    deck = Deck.colorless,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true,
                },
                Name = Localization.ship_Spica_card_DodgeOrShift_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        CardData result = upgrade switch
        {
            Upgrade.A => new() { cost = 0, floppable = true },
            Upgrade.B => new() { cost = 1, floppable = true },
            _ => new() { cost = 1, floppable = true },
        };
        result.art = this.flipped
            ? Sprites.cards_DodgeOrShift_Bottom.Sprite
            : Sprites.cards_DodgeOrShift_Top.Sprite;
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A =>
            [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = !flipped,
                },
            ],
            Upgrade.B =>
            [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 2,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 2,
                    targetPlayer = true,
                    disabled = !flipped,
                },
            ],
            _ =>
            [
                new AStatus()
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AStatus()
                {
                    status = Status.droneShift,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = !flipped,
                },
            ],
        };
    }
}
