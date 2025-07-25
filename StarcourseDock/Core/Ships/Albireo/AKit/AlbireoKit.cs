using System.Runtime.InteropServices;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal static class CardTypeCache
{
    private static Dictionary<CacheCard, Card> singleCardCache = new Dictionary<CacheCard, Card>();

    public static Card CopyOrGet(Card card, bool orange)
    {
        var cardType = new CacheCard(card.upgrade, card.GetType(), orange);
        ref Card? c = ref CollectionsMarshal.GetValueRefOrAddDefault(singleCardCache, cardType, out bool exists);

        if (!exists)
        {
            c = card.CopyWithNewId();
            c.flipAnim = 0;
        }

        return c!;
    }

    private record struct CacheCard(Upgrade Upgrade, Type Type, bool Orange);
}

internal sealed class AlbireoKit : IRegisterable
{
    internal static ICardTraitEntry PolarityTrait { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        PolarityTrait = helper.Content.Cards.RegisterTrait(
            "Polarity",
            new()
            {
                Name = Localization.ship_Albireo_cardtrait_Polarity_name(),
                Icon = static (s, c) => GetIcon(c),
                Tooltips = (state, card) =>
                {
                    if (card is null)
                    {
                        return [GetPolarityTraitTooltip(), Polarity.GetTooltip()];
                    }

                    if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkedCard) && linkedCard is not null)
                    {
                        var cachedCard = CardTypeCache.CopyOrGet(linkedCard, ModEntry.Instance.Helper.ModData.GetModDataOrDefault(card, "polarity.orange", false));
                        return [GetPolarityTraitTooltip(), new TTCard() { card = cachedCard }, Polarity.GetTooltip()];
                    }

                    return [GetPolarityTraitTooltip(), Polarity.GetTooltip()];
                },
            }
        );
    }

    private static Spr? GetIcon(Card? c)
    {
        if (c is null)
        {
            return Sprites.icons_polarity.Sprite;
        }

        if (ModEntry.Instance.Helper.ModData.TryGetModData(c, "polarity.orange", out bool orange))
        {
            if (orange)
            {
                return Sprites.icons_polarity_orange.Sprite;
            }

            return Sprites.icons_polarity_blue.Sprite;
        }

        return Sprites.icons_polarity.Sprite;
    }

    public static Tooltip GetPolarityTraitTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::Polarity")
        {
            Title = Localization.Str_ship_Albireo_cardtrait_Polarity_name(),
            TitleColor = Colors.cardtrait,
            Description = Localization.Str_ship_Albireo_cardtrait_Polarity_description(),
            Icon = Sprites.icons_polarity.Sprite,
        };
    }
}