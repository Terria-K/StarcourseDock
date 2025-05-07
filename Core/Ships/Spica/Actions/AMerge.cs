namespace Teuria.StarcourseDock;

internal class AMerge : CardAction
{
    public bool flipped;

    public override void Begin(G g, State s, Combat c)
    {
        if (flipped)
        {
            s.ship.xLerped = s.ship.x + 1;
            s.ship.x += 1;
        }
        var list = new List<Part>();
        foreach (var p in s.ship.parts)
        {
            if (p.key != "toRemove") 
            {
                list.Add(p);
            }
        }

        s.ship.parts.Clear();
        s.ship.parts = list;
    }
}
