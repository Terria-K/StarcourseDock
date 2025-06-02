using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class BarrageMode : Card, IRegisterable
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
                    rarity = Rarity.rare,
                    dontOffer = true,
                },
                Art = StableSpr.cards_Terminal,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "card", "BarrageMode", "name"]
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
            flippable = true,
            cost = 1,
            description = ModEntry.Instance.Localizations.Localize(
                ["ship", "Sirius", "card", "BarrageMode", "description"]
            ),
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [new AActivateAllParts() { partType = PType.missiles }];
    }

    public override void OnFlip(G g)
    {
        Combat? c = g.state.route as Combat;
        if (c != null)
        {
            c.QueueImmediate(new AToggleMissileBay());
        }
    }
}
