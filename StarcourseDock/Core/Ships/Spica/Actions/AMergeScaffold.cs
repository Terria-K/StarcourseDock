namespace Teuria.StarcourseDock;

internal class AMergeScaffold : CardAction
{
    public bool flipped;

    public override void Begin(G g, State s, Combat c)
    {
        if (!s.ship.IsPartExists("toRemove"))
        {
            return;
        }

        string centerKey;
        if (flipped)
        {
            centerKey = "rightwing";
        }
        else
        {
            centerKey = "leftwing";
        }

        s.ship.RemoveParts(centerKey, ["toRemove"]);
    }
}
