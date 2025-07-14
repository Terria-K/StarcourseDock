using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class Unfreeze : Card, IHasCustomCardTraits
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
                    deck = GlieseKit.GlieseDeck.Deck,
                    rarity = Rarity.uncommon,
                    dontOffer = true,
                    upgradesTo = [Upgrade.A, Upgrade.B],
                },
                Art = StableSpr.cards_SolarFlair,
                Name = Localization.ship_Gliese_card_Unfreeze_name(),
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
                Upgrade.A => Localization.Str_ship_Gliese_card_Unfreeze_A_description(),
                Upgrade.B => Localization.Str_ship_Gliese_card_Unfreeze_B_description(),
                _ => Localization.Str_ship_Gliese_card_Unfreeze_description(),
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

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        return new HashSet<ICardTraitEntry>() { GlieseKit.CantBeFrozenTrait };
    }
}
