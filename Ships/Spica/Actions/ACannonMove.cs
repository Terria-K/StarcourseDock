namespace Teuria.StarcourseDock;

internal class ACannonMove : CardAction
{
    public int dir;

    public override void Begin(G g, State s, Combat c)
    {
        var list = new List<Part>();
        int cannonIndex = 0;
        int i = 0;
        foreach (var p in s.ship.parts)
        {
            if (p.key == "closeToScaffold") 
            {
                cannonIndex = i;
            }
            else 
            {
                list.Add(p);
            }
            i += 1;
        }

        s.ship.parts.Clear();
        s.ship.parts = list;
        s.ship.parts.Insert(cannonIndex + dir, new Part() 
        {
            type = PType.cannon,
            skin = SpicaShip.SpicaCannon.UniqueName,
            key = "closeToScaffold"
        });
    }
}