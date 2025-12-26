namespace Teuria.StarcourseDock;

internal sealed class MissileLaunchFX : FX
{
    public Part? part;
    public int worldX;
    public bool hitShip;
    public bool hitDrone;

    public override void Render(G g, Vec v)
    {
        if (part == null)
        {
            return;
        }

        Spr? spr = Sprites.parts_wolf_rayet_missiles.Sprite;

        double y = v.y - Ease.BackOut(age) * 300 + 32 + 60;

        if (hitShip)
        {
            if (y > 80)
            {
                Draw.Sprite(spr, v.x + worldX + 7, y, originPx: new Vec(8, 32));
            }
        }
        else if (hitDrone)
        {
            if (y > 90)
            {
                Draw.Sprite(spr, v.x + worldX + 7, y, originPx: new Vec(8, 32));
            }
        }
        else
        {
            Draw.Sprite(spr, v.x + worldX + 7, y, originPx: new Vec(8, 32));
        }
    }
}
