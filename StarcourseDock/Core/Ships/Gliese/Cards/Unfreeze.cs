using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class Unfreeze : Card, IRegisterable
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
                    upgradesTo = [Upgrade.A, Upgrade.B],
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
            description = upgrade switch
            {
                Upgrade.A => ModEntry.Instance.Localizations.Localize(
                    ["ship", "Gliese", "card", "Unfreeze", "A", "description"]
                ),
                Upgrade.B => ModEntry.Instance.Localizations.Localize(
                    ["ship", "Gliese", "card", "Unfreeze", "B", "description"]
                ),
                _ => ModEntry.Instance.Localizations.Localize(
                    ["ship", "Gliese", "card", "Unfreeze", "description"]
                ),
            },
        };
        return result;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A =>
            [
                new ADelay() { time = -0.5 },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard() { shouldDraw = true },
                    browseSource = CardBrowse.Source.DiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard() { shouldDraw = true },
                    browseSource = CardBrowse.Source.DiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard() { shouldDraw = true },
                    browseSource = CardBrowse.Source.DiscardPile,
                },
            ],
            Upgrade.B =>
            [
                new ADelay() { time = -0.5 },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DrawOrDiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DrawOrDiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DrawOrDiscardPile,
                },
            ],
            _ =>
            [
                new ADelay() { time = -0.5 },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DiscardPile,
                },
                new ACardSelect()
                {
                    browseAction = new AUnfreezeCard(),
                    browseSource = CardBrowse.Source.DiscardPile,
                },
            ],
        };
    }
}
