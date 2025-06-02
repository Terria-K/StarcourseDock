using System.Reflection;
using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "card", "ToggleMissileBay", "name"]
                    )
                    .Localize,
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
            new AToggleMissileBay(),
            new AStatus()
            {
                status = BayPowerDownStatus.BayPowerDownEntry.Status,
                statusAmount = 1,
                targetPlayer = true,
            },
        ];
    }
}
