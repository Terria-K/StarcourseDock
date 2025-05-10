namespace Teuria.StarcourseDock;

internal class ASwapScaffold : CardAction 
{
    public bool isRight;

    public override void Begin(G g, State s, Combat c)
    {
        ModEntry.Instance.Helper.ModData.SetModData(s, "alpherg_chassis.activation", isRight);
        int len = s.ship.parts.Count;
        var retained = new List<Part>(len);
        if (isRight)
        {
            int i;
            int x = -1;
            for (i = 0; i < len; i += 1)
            {
                var part = s.ship.parts[i];
                if (part.skin != AlphergShip.AlphergScaffoldOrange.UniqueName)
                {
                    retained.Add(part);
                }
                else 
                {
                    x = i;
                }
            }

            s.ship.parts = retained;
            s.ship.InsertPart(s, len - x - 1, len - x - 2, false, new Part() {
                skin = AlphergShip.AlphergScaffoldBlue.UniqueName,
                type = PType.empty
            });
        }
        else 
        {
            int i;
            int x = -1;
            for (i = 0; i < len; i += 1)
            {
                var part = s.ship.parts[i];
                if (part.skin != AlphergShip.AlphergScaffoldBlue.UniqueName)
                {
                    retained.Add(part);
                }
                else 
                {
                    x = i;
                }
            }

            s.ship.parts = retained;
            s.ship.InsertPart(s, Math.Abs(x - len + 1), 0, false, new Part() {
                skin = AlphergShip.AlphergScaffoldOrange.UniqueName,
                type = PType.empty
            });
        }
    }
}