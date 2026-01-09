using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AtlasKit : IRegisterable
{
    internal static ICardTraitEntry TurnEndTriggerTrait { get; private set; } = null!;
    internal static ICardTraitEntry OnDrawTriggerTrait { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        TurnEndTriggerTrait = helper.Content.Cards.RegisterTrait(
            "TurnEndTrigger",
            new()
            {
                Name = Localization.ship_Atlas_cardtrait_TurnEndTrigger_name(),
                Icon = (s, c) => Sprites.icons_turn_end_trigger.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::TurnEndTriggerTrait"
                        )
                        {
                            Title = Localization.Str_ship_Atlas_cardtrait_TurnEndTrigger_name(),
                            TitleColor = Colors.cardtrait,
                            Description = Localization.Str_ship_Atlas_cardtrait_TurnEndTrigger_description(),
                            Icon = Sprites.icons_turn_end_trigger.Sprite,
                        },
                    ];
                },
            }
        );

        OnDrawTriggerTrait = helper.Content.Cards.RegisterTrait(
            "OnDrawTrigger",
            new()
            {
                Name = Localization.ship_Atlas_cardtrait_OnDrawTrigger_name(),
                Icon = (s, c) => Sprites.icons_on_draw_trigger.Sprite,
                Tooltips = (state, card) =>
                {
                    return
                    [
                        new GlossaryTooltip(
                            $"{ModEntry.Instance.Package.Manifest.UniqueName}::OnDrawTriggerTrait"
                        )
                        {
                            Title = Localization.Str_ship_Atlas_cardtrait_OnDrawTrigger_name(),
                            TitleColor = Colors.cardtrait,
                            Description = Localization.Str_ship_Atlas_cardtrait_OnDrawTrigger_description(),
                            Icon = Sprites.icons_on_draw_trigger.Sprite,
                        },
                    ];
                },
            }
        );
    }
}
