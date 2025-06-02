using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class Unfreeze : Card, IRegisterable
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
                    rarity = Rarity.uncommon,
                    dontOffer = true,
                },
                Art = StableSpr.cards_SolarFlair,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "card", "Unfreeze", "name"])
                    .Localize,
            }
        );
    }

    public override CardData GetData(State state)
    {
        CardData result = new()
        {
            cost = 1,
            description = ModEntry.Instance.Localizations.Localize(
                ["ship", "Gliese", "card", "Unfreeze", "description"]
            ),
        };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [new AUnfreezeCard()];
    }
}
