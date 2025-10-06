using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class IoDrone : JupiterDrone
{
    public override void Render(G g, Vec v)
    {
        DrawWithHilight(
            g, Sprites.drones_ioDrone.Sprite, v + GetOffset(g, false), false, targetPlayer);
    }

    public override Spr? GetIcon()
    {
        return Sprites.icons_ioDrone.Sprite;
    }

    public override List<Tooltip> GetTooltips()
    {
        List<Tooltip> ttItems = [];

        ttItems.Add(
            new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::IoDrone")
            {
                Title = Localization.Str_ship_Io_midrow_IoDrone_name(),
                TitleColor = Colors.midrow,
                Description = Localization.Str_ship_Io_midrow_IoDrone_description(),
                Icon = Sprites.icons_ioDrone.Sprite
            }
        );

        if (bubbleShield)
        {
            ttItems.Add(new TTGlossary("midrow.bubbleShield", [])); 
        }

        return ttItems;
    }
}
