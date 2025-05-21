namespace Teuria.StarcourseDock;

internal class ASwapScaffold : CardAction 
{
    public bool isRight;

    public override void Begin(G g, State s, Combat c)
    {
        ModEntry.Instance.Helper.ModData.SetModData(c, "alpherg_chassis.activation", isRight);
        if (isRight)
        {
            int x = s.ship.FindPartIndex(x => x.skin == AlphergShip.AlphergScaffoldOrange.UniqueName);
            if (x == 0)
            {
                return;
            }
            s.ship.parts = s.ship.RetainParts(x => x.skin != AlphergShip.AlphergScaffoldOrange.UniqueName);

            int len = s.ship.parts.Count;

            s.ship.InsertPart(s, len - 1 - x, 0, true, new Part() {
                skin = AlphergShip.AlphergScaffoldBlue.UniqueName,
                type = PType.empty
            });
        }
        else 
        {
            int x = s.ship.FindPartIndex(x => x.skin == AlphergShip.AlphergScaffoldBlue.UniqueName);
            if (x == 0)
            {
                return;
            }
            s.ship.parts = s.ship.RetainParts(x => x.skin != AlphergShip.AlphergScaffoldBlue.UniqueName);

            int len = s.ship.parts.Count;

            s.ship.InsertPart(s, Math.Abs(x - len), 0, false, new Part() {
                skin = AlphergShip.AlphergScaffoldOrange.UniqueName,
                type = PType.empty
            });
        }
    }
}