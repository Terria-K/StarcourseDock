
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class RayetMiniMissile : Missile
{
    private const string MissileName = "rayetMiniMissile";
    private const int BaseDamage = 4;
    public bool heater;

    static RayetMiniMissile()
    {
        DB.drones[MissileName] = Sprites.drones_rayet_mini_missile.Sprite;
    }

    public override double GetWiggleAmount() => 1.8;
    public override double GetWiggleRate() => 3.0;
    public override string GetDialogueTag() => MissileName;

    public override void Render(G g, Vec v)
    {
        Vec offset = GetOffset(g, true);
        Vec wiggle = new Vec(
            Math.Sin(x + g.state.time * 10.0),
            Math.Cos(x + g.state.time * 20.0 + 1.5707963267948966))
            .round();

        offset += wiggle;

        Vec mp = v + offset;

        Vec exhaustOffset = new Vec(0.0, 21.0);

        Vec exhaustSpriteOrigin = mp + exhaustOffset + new Vec(7.0, 8.0);
        Spr exhaustSprites = Missile.exhaustSprites.GetModulo((int)(g.state.time * 36.0 + x * 10));

        Color exhaustColor = new Color("ff5b21");
        DrawWithHilight(g, heater ? Sprites.drones_rayet_mini_missile_A.Sprite : Sprites.drones_rayet_mini_missile.Sprite, mp, false, targetPlayer);
        Draw.Sprite(exhaustSprites, exhaustSpriteOrigin.x - 5.0, exhaustSpriteOrigin.y + (targetPlayer ? 0 : 14), flipY: !targetPlayer, originRel: new Vec(0.0, 1.0), color: exhaustColor);
        Glow.Draw(exhaustSpriteOrigin + new Vec(0.5, -2.5), 25.0, exhaustColor * new Color(1.0, 0.5, 0.5).gain(0.2 + 0.1 * Math.Sin(g.state.time * 30.0 + x) * 0.5));
    }
    

    public override Spr? GetIcon()
    {
        return heater ? Sprites.icons_rayet_mini_missile_A.Sprite : Sprites.icons_rayet_mini_missile.Sprite;
    }

    public override List<Tooltip> GetTooltips()
    {
        return [
            new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::RayetMiniMissile")
            {
                Title = !heater
                    ? Localization.Str_ship_WolfRayet_midrow_RayetMiniMissile_name()
                    : Localization.Str_ship_WolfRayet_midrow_RayetMiniMissile_A_name(),
                TitleColor = Colors.midrow,
                Description = !heater
                    ? Localization.Str_ship_WolfRayet_midrow_RayetMiniMissile_description()
                    : Localization.Str_ship_WolfRayet_midrow_RayetMiniMissile_A_description(),
                Icon = heater ? Sprites.icons_rayet_mini_missile_A.Sprite : Sprites.icons_rayet_mini_missile.Sprite
            }
        ];
    }


    public override List<CardAction>? GetActions(State s, Combat c)
    {
        List<CardAction> list = [
            new AMissileHit()
            {
                worldX = x,
                outgoingDamage = BaseDamage,
                targetPlayer = targetPlayer
            }
        ];

        if (heater)
        {
            list.Add(new AStatus() { targetPlayer = targetPlayer, status = Status.heat, statusAmount = 3 });
        }

        return list;
    }
    
}