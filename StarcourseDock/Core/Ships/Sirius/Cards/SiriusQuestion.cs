using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class SiriusQuestion : Card, IRegisterable
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
                Name = Localization.ship_Sirius_card_SiriusQuestion_name(),
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
            flippable = upgrade == Upgrade.B
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A => [new ASpawn { thing = new SiriusSemiDualDrone() }, new AStatus() { targetPlayer = true, status = Status.droneShift, statusAmount = 1 }],
            Upgrade.B => [
                new ASpawn { thing = new SiriusSemiDualDrone() { bubbleShield = true } }
            ],
            _ => [new ASpawn { thing = new SiriusSemiDualDrone() }],
        };
    }
}
