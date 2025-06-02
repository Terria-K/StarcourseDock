using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class MissileDeliveryFX : FX
{
    public Part? part;
    public int worldX;

    public override void Update(G g)
    {
        base.Update(g);
        if (IsDone)
        {
            if (g.state is not null && g.state.route is Combat c && part is not null)
            {
                part.active = false;
                part.skin = WolfRayetShip.MissileSlot.UniqueName;
                part.type = WolfRayetShip.MissilePartType;
                part.stunModifier = WolfRayetShip.HotStunModifier;
            }
        }
    }

    public override void Render(G g, Vec v)
    {
        if (part == null)
        {
            return;
        }

        Spr? spr = Sprites.parts_wolf_rayet_missiles_inactive.Sprite;

        double y = v.y + Ease.SineIn(1 - age) * 300 + 32 + 80;

        Draw.Sprite(spr, v.x + worldX + 7, y, originPx: new Vec(8, 32));
    }
}
