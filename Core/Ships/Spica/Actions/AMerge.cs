namespace Teuria.StarcourseDock;

internal class AMerge : CardAction
{
    public bool flipped;
    public Part? part;

    public override void Begin(G g, State s, Combat c)
    {
        var artifact = s.GetArtifact<ShrinkMechanismV2>();
        if (artifact == null)
        {
            return;
        }

        artifact.leftParts ??= new List<Part>();
        artifact.rightParts ??= new List<Part>();

        part ??= flipped ? s.ship.GetPartByKey("leftwing") : s.ship.GetPartByKey("rightwing");

        if (part == null)
        {
            return;
        }

        switch (part.key)
        {
            case "rightwing":
                int? rx = s.ship.GetLocalXOfPart("rightwing");
                if (rx != null)
                {
                    int v = rx.Value;
                    Part? p = s.ship.GetPartAtLocalX(v - 1);
                    if (p != null && p.type != PType.wing)
                    {
                        artifact.rightParts.Add(p);
                        s.ship.RemoveParts("leftwing", [p.key]);
                    }
                }
                break;
            case "leftwing":
                int? lx = s.ship.GetLocalXOfPart("leftwing");
                if (lx != null)
                {
                    int v = lx.Value;
                    Part? p = s.ship.GetPartAtLocalX(v + 1);
                    if (p != null && p.type != PType.wing)
                    {
                        artifact.leftParts.Add(p);
                        s.ship.RemoveParts("rightwing", [p.key]);
                    }
                }
                break;
        }
    }
}
