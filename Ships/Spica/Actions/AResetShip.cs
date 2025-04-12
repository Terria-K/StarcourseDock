namespace Teuria.StarcourseDock;

internal class AResetShip : CardAction
{
    public List<Part>? parts;

    public override void Begin(G g, State s, Combat c)
    {
        if (parts is not null)
        {
            s.ship.parts = parts;
        }
    }
}