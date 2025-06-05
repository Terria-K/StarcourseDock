using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class Shrink : Card, IRegisterable
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
                    deck = SpicaKit.SpicaDeck.Deck,
                    rarity = Rarity.common,
                    upgradesTo = [Upgrade.A],
                    dontOffer = true,
                },
                Art = StableSpr.cards_ScootLeft,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Spica", "card", "Shrink", "name"])
                    .Localize,
            }
        );
    }

    public override CardData GetData(State state)
    {
        var cardData = new CardData()
        {
            temporary = true,
            flippable = true,
            retain = true,
            cost = 1,
            art = flipped ? StableSpr.cards_ScootRight : StableSpr.cards_ScootLeft,
        };
        return upgrade switch
        {
            Upgrade.A => cardData with
            {
                infinite = true,
                description = flipped
                    ? ModEntry.Instance.Localizations.Localize(
                        ["ship", "Spica", "card", "Shrink_Flipped", "A", "description"]
                    )
                    : ModEntry.Instance.Localizations.Localize(
                        ["ship", "Spica", "card", "Shrink", "A", "description"]
                    ),
            },
            _ => cardData with
            {
                singleUse = true,
                description = flipped
                    ? ModEntry.Instance.Localizations.Localize(
                        ["ship", "Spica", "card", "Shrink_Flipped", "description"]
                    )
                    : ModEntry.Instance.Localizations.Localize(
                        ["ship", "Spica", "card", "Shrink", "description"]
                    ),
            },
        };
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A => [new AMerge() { flipped = flipped }],
            _ => [new AMergeScaffold() { flipped = flipped }],
        };
    }
}
