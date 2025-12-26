using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class ALaunchMissiles : CardAction
{
    public int reduceDamage;
    public bool isPlayerShip;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 1;
        Ship target = isPlayerShip ? s.ship : c.otherShip;

        for (int i = 0; i < target.parts.Count; i++)
        {
            Part part = target.parts[i];
            if (part == null)
            {
                continue;
            }

            if (part.type == WolfRayetShip.MissilePartType.PartType && part.active)
            {
                c.Queue(
                    new ALaunchMissile()
                    {
                        part = part,
                        localX = i,
                        targetPlayer = isPlayerShip,
                        reduceDamage = reduceDamage
                    }
                );
            }
        }
        timer = 0;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        int damage = 4 - reduceDamage;

        if (s.HasArtifactFromColorless<SeriousDedication>())
        {
            damage = 3;
        }

        return [
            new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::LaunchMissiles")
            {
                Title = Localization.Str_ship_WolfRayet_action_Launch_name(),
                TitleColor = Colors.action,
                Description = Localization.Str_ship_WolfRayet_action_Launch_description(damage.ToString()),
                Icon = Sprites.icons_launch.Sprite
            }
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(Sprites.icons_launch.Sprite, null, Colors.white);
    }
}
