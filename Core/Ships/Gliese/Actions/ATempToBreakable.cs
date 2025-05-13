using FSPRO;

namespace Teuria.StarcourseDock;

internal class ATempToBreakable : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        for (int i = 0; i < s.ship.parts.Count; i++)
        {
            var part = s.ship.parts[i];
            if (part.key == null || part.key != "crystal_tempcannon::StarcourseDock")
            {
                continue;
            }

            s.ship.parts[i] = new Part() 
            {
                type = PType.special,
                skin = GlieseShip.GlieseCrystal2.UniqueName,
                stunModifier = PStunMod.breakable,
                key = "crystal2::StarcourseDock"
            };
        }

        Audio.Play(Event.Status_PowerDown);
    }
}