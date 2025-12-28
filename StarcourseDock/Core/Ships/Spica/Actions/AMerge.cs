namespace Teuria.StarcourseDock;

internal class AMerge : CardAction
{
    public bool flipped;
    public Part? part;

    public override void Begin(G g, State s, Combat c)
    {
        var artifact = s.GetArtifactFromColorless<ShrinkMechanismV2>();
        if (artifact == null)
        {
            return;
        }

        artifact.leftParts ??= new List<Part>();
        artifact.rightParts ??= new List<Part>();

        part ??= flipped ? s.ship.GetPartByKey("Starcourse::leftwing") : s.ship.GetPartByKey("Starcourse::rightwing");

        if (part == null)
        {
            return;
        }

        switch (part.key)
        {
            case "Starcourse::rightwing":
                int? rx = s.ship.GetLocalXOfPart("Starcourse::rightwing");
                if (rx != null)
                {
                    int v = rx.Value;
                    Part? p = s.ship.GetPartAtLocalX(v - 1);
                    if (p != null && p.type != PType.wing)
                    {
                        artifact.rightParts.Add(p);
                        s.ship.RemoveParts("Starcourse::leftwing", [p.key!]);
                    }
                }
                break;
            case "Starcourse::leftwing":
                int? lx = s.ship.GetLocalXOfPart("Starcourse::leftwing");
                if (lx != null)
                {
                    int v = lx.Value;
                    Part? p = s.ship.GetPartAtLocalX(v + 1);
                    if (p != null && p.type != PType.wing)
                    {
                        artifact.leftParts.Add(p);
                        s.ship.RemoveParts("Starcourse::rightwing", [p.key!]);
                    }
                }
                break;
        }
    }
}
