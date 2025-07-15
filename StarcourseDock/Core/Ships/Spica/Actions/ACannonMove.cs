using FMOD;
using FSPRO;

namespace Teuria.StarcourseDock;

internal class ACannonMove : CardAction
{
    public int dir;
    public bool ignoreHermes;
    public bool preferRightWhenZero;

    public override void Begin(G g, State s, Combat c)
    {
        int hermes = s.ship.Get(Status.hermes);
        if (hermes > 0 && !ignoreHermes)
        {
            if (dir == 0 && preferRightWhenZero)
            {
                dir += hermes;
            }
            else
            {
                dir += ((dir > 0) ? hermes : hermes * -1);
            }
        }

        int cannonIndex = s.ship.FindPartIndex("closeToScaffold");
        var list = s.ship.RetainParts(p => p.key != "closeToScaffold");

        int index = 0;

        if (!s.HasArtifactFromColorless<TinyWormhole>())
        {
            index = Mutil.Clamp(cannonIndex + dir, 1, s.ship.parts.Count - 2);
        }
        else
        {
            index = MathUtils.Wrap(cannonIndex + dir, 1, s.ship.parts.Count - 1);
        }

        s.ship.parts.Clear();
        s.ship.parts = list;
        s.ship.parts.Insert(
            index,
            new Part()
            {
                type = PType.cannon,
                skin = SpicaShip.SpicaCannon.UniqueName,
                key = "closeToScaffold",
            }
        );

        int strafe = s.ship.Get(Status.strafe);

        if (strafe > 0)
        {
            c.QueueImmediate(
                new AAttack
                {
                    damage = Card.GetActualDamage(s, strafe, false, null),
                    targetPlayer = false,
                    fast = true,
                    storyFromStrafe = true,
                }
            );
        }
    }
}
