namespace Teuria.StarcourseDock;

internal class ARepairAllBrokenPart : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        foreach (var part in s.ship.parts)
        {
            if (part.key == null || !part.key.StartsWith("crystal"))
            {
                continue;
            }
            switch (part.key)
            {
                case "crystal1::StarcourseDock":
                    part.skin = "crystal_1";
                    break;
                case "crystal2::StarcourseDock":
                    part.skin = "crystal_2";
                    break;
                case "crystal3::StarcourseDock":
                    part.skin = "crystal_3";
                    break;
                case "crystal4::StarcourseDock":
                    part.skin = "crystal_0";
                    break;
            }

            part.stunModifier = PStunMod.breakable;
            part.type = PType.special;
        }
    }
}
