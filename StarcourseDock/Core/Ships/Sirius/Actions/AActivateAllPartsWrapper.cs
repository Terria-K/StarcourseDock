using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AActivateAllPartsWrapper : CardAction
{
    public PType partType;
    public bool targetPlayer = true;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        c.QueueImmediate(new AActivateAllParts() { partType = partType });
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(Sprites.icons_sirius_barrage.Sprite, null, Colors.white);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        Ship ship = (!(s.route is Combat combat)) ? s.ship : (targetPlayer ? s.ship : combat.otherShip);
        foreach (Part part in ship.parts)
        {
            if (part.type == partType)
            {
                part.hilightToggle = true;
            }
        }
        return
        [
            new GlossaryTooltip(
                $"{ModEntry.Instance.Package.Manifest.UniqueName}::BarrageIcon"
            )
            {
                Title = Localization.Str_ship_Sirius_icon_barrageMode_name(),
                Description = Localization.Str_ship_Sirius_icon_barrageMode_description(),
                TitleColor = Colors.action,
                IsWideIcon = true,
                Icon = Sprites.icons_sirius_barrage.Sprite,
            },
        ];
    }
}