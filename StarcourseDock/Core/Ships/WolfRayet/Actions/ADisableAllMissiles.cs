namespace Teuria.StarcourseDock;

internal sealed class ADisableAllMissiles : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        for (int i = 0; i < s.ship.parts.Count; i++)
        {
            var part = s.ship.parts[i];
            if (part.type == WolfRayetShip.MissilePartType)
            {
                part.active = false;
            }
        }
    }
}
