namespace Teuria.StarcourseDock;

internal sealed class ALaunchMissiles : CardAction
{
    public bool targetPlayer;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 1;
        global::Ship target = (targetPlayer ? s.ship : c.otherShip);

        for (int i = 0; i < target.parts.Count; i++)
        {
            Part part = target.parts[i];
            if (part == null)
            {
                continue;
            }

            if (part.type == WolfRayetShip.MissilePartType && part.active)
            {
                c.Queue(
                    new ALaunchMissile()
                    {
                        part = part,
                        localX = i,
                        targetPlayer = targetPlayer,
                    }
                );
            }
        }
        timer = 0;
    }
}
