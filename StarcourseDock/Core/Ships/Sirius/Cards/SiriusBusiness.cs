using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "card", "SiriusBusiness", "name"]
                    )
                    .Localize,
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
