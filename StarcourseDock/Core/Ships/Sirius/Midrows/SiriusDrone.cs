using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusDrone : JupiterDrone
{
    public static new List<string> droneNames = new List<string>
    {
        "sirius",
        "spica",
        "eta",
        "tinylittlegoblin",
        "brightjr",
        "isaacjr",
    };
    public Upgrade upgrade;

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
            upgrade switch
            {
                Upgrade.A => Sprites.drones_siriusDroneMKII.Sprite,
                _ => Sprites.drones_siriusDrone.Sprite,
            },
            offset
        );
    }

    public override List<Tooltip> GetTooltips()
    {
        List<Tooltip> ttItems =
        [
            upgrade switch
            {
                Upgrade.A => new GlossaryTooltip(
                    $"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusDrone"
                )
                {
                    Title = Localization.Str_ship_Sirius_midrow_SiriusDroneMKII_name(),
                    Description = Localization.Str_ship_Sirius_midrow_SiriusDroneMKII_description(),
                    TitleColor = Colors.midrow,
                    Icon = Sprites.icons_siriusDroneMkII.Sprite,
                    flipIconY = targetPlayer,
                },
                _ => new GlossaryTooltip(
                    $"{ModEntry.Instance.Package.Manifest.UniqueName}::siriusDrone"
                )
                {
                    Title = Localization.Str_ship_Sirius_midrow_SiriusDrone_name(),
                    Description = Localization.Str_ship_Sirius_midrow_SiriusDrone_description(),
                    TitleColor = Colors.midrow,
                    Icon = Sprites.icons_siriusDrone.Sprite,
                    flipIconY = targetPlayer,
                },
            },
        ];

        if (bubbleShield)
        {
            ttItems.Add(new TTGlossary("midrow.bubbleShield", Array.Empty<object>()));
        }
        return ttItems;
    }

    public override Spr? GetIcon()
    {
        return upgrade switch
        {
            Upgrade.A => Sprites.icons_siriusDroneMkII.Sprite,
            _ => Sprites.icons_siriusDrone.Sprite,
        };
    }
}
