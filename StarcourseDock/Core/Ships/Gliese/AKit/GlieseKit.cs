using System.Runtime.InteropServices;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GlieseKit : IRegisterable
{
    internal static ICardTraitEntry FrozenTrait { get; set; } = null!;
    internal static ICardTraitEntry CantBeFrozenTrait { get; set; } = null!;
    internal static ICardTraitEntry TurnEndTriggerTrait { get; set; } = null!;
    private static Dictionary<int, Spr> frozenSpritesCache = new Dictionary<int, Spr>();

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        FrozenTrait = helper.Content.Cards.RegisterTrait(
            "Frozen",
            new()
            {
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "trait", "Frozen", "name"])
                    .Localize,
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "trait", "Frozen", "name"])
                    .Localize,
                Icon = (State s, Card? c) => Sprites.icons_cant_frozen.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::CantBeFrozenTrait"
                        )
                        {
                            Title = ModEntry.Instance.Localizations.Localize(
                                ["ship", "Gliese", "trait", "CantBeFrozen", "name"]
                            ),
                            TitleColor = Colors.cardtrait,
                            Description = ModEntry.Instance.Localizations.Localize(
                                ["ship", "Gliese", "trait", "CantBeFrozen", "description"]
                            ),
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "trait", "Frozen", "name"])
                    .Localize,
                Icon = (State s, Card? c) => Sprites.icons_turn_end_trigger.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::TurnEndTriggerTrait"
                        )
                        {
                            Title = ModEntry.Instance.Localizations.Localize(
                                ["ship", "Gliese", "trait", "TurnEndTrigger", "name"]
                            ),
                            TitleColor = Colors.cardtrait,
                            Description = ModEntry.Instance.Localizations.Localize(
                                ["ship", "Gliese", "trait", "TurnEndTrigger", "description"]
                            ),
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
            Title = ModEntry.Instance.Localizations.Localize(
                ["ship", "Gliese", "trait", "Frozen", "name"]
            ),
            TitleColor = Colors.cardtrait,
            Description = ModEntry.Instance.Localizations.Localize(
                ["ship", "Gliese", "trait", "Frozen", "description"]
            ),
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
