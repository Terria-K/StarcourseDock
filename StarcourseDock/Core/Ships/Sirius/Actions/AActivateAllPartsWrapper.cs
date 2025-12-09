using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AActivateAllPartsWrapper : CardAction
{
    public PType partType;
    public bool targetPlayer = true;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        c.QueueImmediate(new AActivateAllParts() { partType = partType });
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        Ship ship = (!(s.route is Combat combat)) ? s.ship : (targetPlayer ? s.ship : combat.otherShip);
        foreach (Part part in ship.parts)
        {
            if (part.type == partType)
            {
                part.hilightToggle = true;
            }
        }
        return [];
    }
}