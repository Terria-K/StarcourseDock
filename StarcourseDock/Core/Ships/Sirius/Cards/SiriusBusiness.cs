using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class SiriusBusiness : Card, IRegisterable
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
                    deck = SiriusKit.SiriusDeck.Deck,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                    dontOffer = true,
                },
                Art = StableSpr.cards_GoatDrone,
                Name = Localization.ship_Sirius_card_SiriusBusiness_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return new CardData()
        {
            recycle = true,
            cost = 1,
            retain = true,
            buoyant = true,
            unremovableAtShops = true,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.B => [new ASpawn { thing = new SiriusDrone() { bubbleShield = true } }],
            _ => [new ASpawn { thing = new SiriusDrone() { upgrade = this.upgrade } }],
        };
    }
}
