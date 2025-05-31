namespace Teuria.StarcourseDock;

internal class ARemoveAllBrokenPart : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        var listToRemove = new List<Part>();
        foreach (var part in s.ship.parts)
        {
            if (part.key == null || !part.key.StartsWith("crystal"))
            {
                continue;
            }

            if (part.type != PType.empty)
            {
                continue;
            }

            listToRemove.Add(part);
        }

        foreach (var part in listToRemove)
        {
            s.ship.parts.Remove(part);
        }
    }
}
