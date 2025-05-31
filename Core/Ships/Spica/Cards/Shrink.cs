using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class Shrink : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var spicaDeck = helper.Content.Decks.RegisterDeck(
            "Spica",
            new()
            {
                Definition = new() { color = new Color("5a7752"), titleColor = Colors.white },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.border_spica.Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "name"]).Localize,
            }
        );

        helper.Content.Cards.RegisterCard(
            MethodBase.GetCurrentMethod()!.DeclaringType!.Name,
            new()
            {
                CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    deck = spicaDeck.Deck,
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
            singleUse = true,
            retain = true,
            temporary = true,
            flippable = true,
            cost = 1,
            art = flipped ? StableSpr.cards_ScootRight : StableSpr.cards_ScootLeft,
        };
        return upgrade switch
        {
            Upgrade.A => cardData with
            {
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
