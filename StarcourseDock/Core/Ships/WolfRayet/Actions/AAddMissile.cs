namespace Teuria.StarcourseDock;

internal sealed class AAddMissile : CardAction
{
    public int x;
    public bool targetPlayer;

    public override void Begin(G g, State s, Combat c)
    {
        Ship target = (targetPlayer ? s.ship : c.otherShip);

        Part? part = target.GetPartAtLocalX(x);

        if (part != null)
        {
            c.fx.Add(new MissileDeliveryFX() { part = part, worldX = (x + s.ship.x) * 16 });
        }
    }
}
