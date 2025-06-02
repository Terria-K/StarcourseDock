using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class RelativeMotion : Card
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
                    dontOffer = true,
                },
                Art = StableSpr.cards_ScootRight,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Albireo", "card", "RelativeMotion", "name"]
                    )
                    .Localize,
            }
        );
    }

    public override CardData GetData(State state)
    {
        CardData result = new()
        {
            singleUse = true,
            temporary = true,
            flippable = true,
            cost = 0,
            art = flipped ? StableSpr.cards_ScootLeft : StableSpr.cards_ScootRight,
        };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [new AMove() { targetPlayer = true, dir = 3 }];
    }
}
