using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class ShieldOrShot : Card, IRegisterable
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
                Name = Localization.ship_Spica_card_ShieldOrShot_name(),
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
            ? Sprites.cards_ShieldOrShot_Bottom.Sprite
            : Sprites.cards_ShieldOrShot_Top.Sprite;
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
                    status = Status.shield,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AAttack() { damage = GetDmg(s, 1, false), disabled = !flipped },
            ],
            Upgrade.B =>
            [
                new AStatus()
                {
                    status = Status.shield,
                    statusAmount = 2,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AAttack() { damage = GetDmg(s, 2, false), disabled = !flipped },
            ],
            _ =>
            [
                new AStatus()
                {
                    status = Status.shield,
                    statusAmount = 1,
                    targetPlayer = true,
                    disabled = flipped,
                },
                new ADummyAction(),
                new AAttack() { damage = GetDmg(s, 1, false), disabled = !flipped },
            ],
        };
    }
}
