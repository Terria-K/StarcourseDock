using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class AFreezeCard : CardAction
{
    public int increment;
    public bool mustHaveTheTrait;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        if (selectedCard == null)
        {
            return;
        }

        var card = selectedCard;

        if (card is Unfreeze)
        {
            return;
        }

        if (
            !ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(
                s,
                card,
                GlieseKit.FrozenTrait
            )
        )
        {
            if (mustHaveTheTrait)
            {
                return;
            }

            ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", 0);
        }

        ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
            s,
            card,
            GlieseKit.FrozenTrait,
            true,
            false
        );

        var frozenCount = ModEntry.Instance.Helper.ModData.GetModDataOrDefault<int>(
            card,
            "FrozenCount"
        );

        int newCount = frozenCount + increment;

        ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", newCount);

        if (newCount > 3)
        {
            card.unplayableOverride = true;
        }
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(Sprites.icons_frozen_3.Sprite, null, Colors.white);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return [GlieseKit.GetFrozenTraitTooltip()];
    }
}

internal sealed class AFreezeCardWrapper : CardAction
{
    public AFreezeCard? action;
    public bool rightSide;

    public override void Begin(G g, State s, Combat c)
    {
        if (action is null)
        {
            return;
        }
        timer = 0.0;
        c.QueueImmediate(action);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        Tooltip modifierTooltip;
        if (rightSide)
        {
            modifierTooltip = new GlossaryTooltip(
                $"{ModEntry.Instance.Package.Manifest.UniqueName}::RightFreeze"
            )
            {
                Title = Localization.Str_ship_Gliese_action_RightFreeze_name(),
                TitleColor = Colors.action,
                Description = Localization.Str_ship_Gliese_action_RightFreeze_description(),
                Icon = Sprites.icons_right_freeze.Sprite,
            };
        }
        else
        {
            modifierTooltip = new GlossaryTooltip(
                $"{ModEntry.Instance.Package.Manifest.UniqueName}::LeftFreeze"
            )
            {
                Title = Localization.Str_ship_Gliese_action_LeftFreeze_name(), 
                TitleColor = Colors.action,
                Description = Localization.Str_ship_Gliese_action_LeftFreeze_description(),
                Icon = Sprites.icons_left_freeze.Sprite,
            };
        }

        return [modifierTooltip, .. action?.GetTooltips(s) ?? []];
    }
}
