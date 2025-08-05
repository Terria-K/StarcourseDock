using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class ToggleMissileBay : Card, IRegisterable
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
                    dontOffer = true,
                },
                Art = StableSpr.cards_GoatDrone,
                Name = Localization.ship_Sirius_card_ToggleMissileBay_name(),
            }
        );
    }

    public override CardData GetData(State state)
    {
        return new()
        {
            temporary = true,
            retain = true,
            singleUse = true,
            cost = 0,
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AToggleMissileBay()
        ];
    }
}
