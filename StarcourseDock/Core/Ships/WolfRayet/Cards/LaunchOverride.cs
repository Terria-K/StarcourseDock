using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class LaunchOverride : Card, IRegisterable
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
                Art = StableSpr.cards_hacker,
                Name = Localization.ship_WolfRayet_card_LaunchOverride_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return upgrade switch
        {
            Upgrade.A => new CardData()
            {
                cost = 1,
                description = Localization.Str_ship_WolfRayet_card_LaunchOverride_description("1"),
                retain = true
            },
            Upgrade.B => new CardData()
            {
                cost = 2,
                description = Localization.Str_ship_WolfRayet_card_LaunchOverride_B_description()
            },
            _ => new CardData()
            {
                cost = 1,
                description = Localization.Str_ship_WolfRayet_card_LaunchOverride_description("1")
            }
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A => [new ALaunchMissiles() { reduceDamage = 1, isPlayerShip = true }],
            Upgrade.B => [new ALaunchMissiles() { isPlayerShip = true }],
            _ => [new ALaunchMissiles() { reduceDamage = 1, isPlayerShip = true }]
        };
    }
}