using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GlieseKit 
{
    internal static ICardTraitEntry FrozenTrait { get; private set; } = null!;
    internal static ICardTraitEntry CantBeFrozenTrait { get; private set; } = null!;
    internal static ICardTraitEntry TurnEndTriggerTrait { get; private set; } = null!;
    internal static IDeckEntry GlieseDeck { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        GlieseDeck = helper.Content.Decks.RegisterDeck(
            "Gliese",
            new()
            {
                Definition = new() { color = new Color("acdfff"), titleColor = Colors.black },
                DefaultCardArt = StableSpr.cards_colorless,
                BorderSprite = Sprites.cardShared_border_gliese.Sprite,
                Name = Localization.ship_Gliese_name(),
            }
        );

        FrozenTrait = helper.Content.Cards.RegisterTrait(
            "Frozen",
            new()
            {
                Name = Localization.ship_Gliese_trait_Frozen_name(),
                Icon = (State s, Card? c) => ObtainFrozenTraitIcon(GetFrozenTraitAmount(c)),
                Tooltips = (state, card) =>
                {
                    return [GetFrozenTraitTooltip()];
                },
            }
        );

        CantBeFrozenTrait = helper.Content.Cards.RegisterTrait(
            "CantBeFrozen",
            new()
            {
                Name = Localization.ship_Gliese_trait_CantBeFrozen_name(),
                Icon = (State s, Card? c) => Sprites.icons_cant_frozen.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::CantBeFrozenTrait"
                        )
                        {
                            Title = Localization.Str_ship_Gliese_trait_CantBeFrozen_name(),
                            TitleColor = Colors.cardtrait,
                            Description = Localization.Str_ship_Gliese_trait_CantBeFrozen_description(),
                            Icon = Sprites.icons_cant_frozen.Sprite,
                        },
                        GetFrozenTraitTooltip(),
                    ];
                },
            }
        );

        TurnEndTriggerTrait = helper.Content.Cards.RegisterTrait(
            "TurnEndTrigger",
            new()
            {
                Name = Localization.ship_Gliese_trait_TurnEndTrigger_name(),
                Icon = (State s, Card? c) => Sprites.icons_turn_end_trigger.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::TurnEndTriggerTrait"
                        )
                        {
                            Title = Localization.Str_ship_Gliese_trait_TurnEndTrigger_name(),
                            TitleColor = Colors.cardtrait,
                            Description = Localization.Str_ship_Gliese_trait_TurnEndTrigger_description(),
                            Icon = Sprites.icons_turn_end_trigger.Sprite,
                        },
                    ];
                },
            }
        );
    }

    public static Tooltip GetFrozenTraitTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::FrozenTrait")
        {
            Title = Localization.Str_ship_Gliese_trait_Frozen_name(),
            TitleColor = Colors.cardtrait,
            Description = Localization.Str_ship_Gliese_trait_Frozen_description(),
            Icon = Sprites.icons_frozen.Sprite,
        };
    }

    private static int GetFrozenTraitAmount(Card? c)
    {
        if (c is null)
        {
            return 0;
        }

        if (ModEntry.Instance.Helper.ModData.TryGetModData(c, "FrozenCount", out int frozenAmount))
        {
            return frozenAmount;
        }

        return 0;
    }

    private static Spr ObtainFrozenTraitIcon(int amount)
    {
        return amount switch
        {
            3 => Sprites.icons_frozen_3.Sprite,
            2 => Sprites.icons_frozen_2.Sprite,
            1 => Sprites.icons_frozen_1.Sprite,
            _ => Sprites.icons_frozen.Sprite,
        };
    }
}
