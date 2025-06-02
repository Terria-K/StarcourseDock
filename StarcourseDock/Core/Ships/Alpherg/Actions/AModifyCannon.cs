namespace Teuria.StarcourseDock;

internal class AModifyCannon : CardAction
{
    public bool active;

    public override void Begin(G g, State s, Combat c)
    {
        foreach (var part in s.ship.parts)
        {
            if (part.type == PType.cannon)
            {
                part.active = active;
            }
        }
    }
}
