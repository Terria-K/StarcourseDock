using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FalseLaunch : Card, IRegisterable
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
                    deck = WolfRayetKit.WolfRayetDeck.Deck,
                    rarity = Rarity.uncommon,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true,
                },
                Art = StableSpr.cards_Meteor,
                Name = Localization.ship_WolfRayet_card_FalseLaunch_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return upgrade switch
        {
            Upgrade.A => new CardData()
            {
                cost = 2,
                exhaust = true
            },
            Upgrade.B => new CardData()
            {
                cost = 3,
                exhaust = true
            },
            _ => new CardData()
            {
                cost = 2,
                exhaust = true
            }
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A => [new ASpawn() { thing = new RayetMiniMissile() }, new AStatus() { targetPlayer = true, status = Status.heat, statusAmount = 1}],
            Upgrade.B => [new ASpawn() { thing = new RayetMiniMissile() { heater = true } }, new AStatus() { targetPlayer = true, status = Status.heat, statusAmount = 2}],
            _ => [new ASpawn() { thing = new RayetMiniMissile() }, new AStatus() { targetPlayer = true, status = Status.heat, statusAmount = 2}],
        };
    }
}
