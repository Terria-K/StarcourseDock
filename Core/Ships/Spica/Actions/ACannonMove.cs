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

        if (s.ship.Get(Status.lockdown) > 0)
        {
            Audio.Play(Event.Status_PowerDown, true);
            s.ship.shake += 1.0;
            return;
        }

        if (s.ship.Get(Status.engineStall) > 0)
        {
            Audio.Play(Event.Status_PowerDown, true);
            s.ship.shake += 1.0;
            s.ship.Add(Status.engineStall, -1);
            return;
        }

        var list = new List<Part>();
        int cannonIndex = 0;
        int i = 0;
        foreach (var p in s.ship.parts)
        {
            if (p.key == "closeToScaffold") 
            {
                cannonIndex = i;
            }
            else 
            {
                list.Add(p);
            }
            i += 1;
        }

        int index = 0;

        if (dir >= 1)
        {
            index = Math.Min(cannonIndex + dir, s.ship.parts.Count - 2);
        }
        else if (dir <= -1)
        {
            index = Math.Max(cannonIndex + dir, 1);
        }

        s.ship.parts.Clear();
        s.ship.parts = list;
        s.ship.parts.Insert(index, new Part() 
        {
            type = PType.cannon,
            skin = SpicaShip.SpicaCannon.UniqueName,
            key = "closeToScaffold"
        });

        int strafe = s.ship.Get(Status.strafe);

        if (strafe > 0)
        {
            c.QueueImmediate(new AAttack
			{
				damage = Card.GetActualDamage(s, strafe, false, null),
				targetPlayer = false,
				fast = true,
				storyFromStrafe = true
			});
        }
    }
}