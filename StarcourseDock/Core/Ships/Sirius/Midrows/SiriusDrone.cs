using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusDrone : JupiterDrone
{
    public static new List<string> droneNames = new List<string>
    {
        "sirius",
        "spica",
        "eta",
        "brightjr",
        "isaacjr",
    };

    public override bool IsHostile()
    {
        return targetPlayer;
    }

    public override List<string> PossibleDroneNames()
    {
        return droneNames;
    }

    public override void Render(G g, Vec v)
    {
        Vec offset = v + GetOffset(g);
        DrawWithHilight(
            g,
            Sprites.drones_siriusDrone.Sprite,
            offset
        );
    }

    public static Tooltip GetGlobalTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusDrone")
        {
            Title = Localization.Str_ship_Sirius_midrow_SiriusDrone_name(),
            Description = Localization.Str_ship_Sirius_midrow_SiriusDrone_description(),
            TitleColor = Colors.midrow,
            Icon = Sprites.icons_siriusDrone.Sprite
        };
    }

    public override List<Tooltip> GetTooltips()
    {
        List<Tooltip> ttItems =
        [
            new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusDrone")
            {
                Title = Localization.Str_ship_Sirius_midrow_SiriusDrone_name(),
                Description = Localization.Str_ship_Sirius_midrow_SiriusDrone_description(),
                TitleColor = Colors.midrow,
                Icon = Sprites.icons_siriusDrone.Sprite,
                flipIconY = targetPlayer,
            },
        ];

        if (bubbleShield)
        {
            ttItems.Add(new TTGlossary("midrow.bubbleShield", []));
        }
        return ttItems;
    }

    public override Spr? GetIcon() => Sprites.icons_siriusDrone.Sprite;
}
