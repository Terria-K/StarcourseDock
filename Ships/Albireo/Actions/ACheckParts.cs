namespace Teuria.StarcourseDock;

internal sealed class ACheckParts : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        var ship = s.ship;
        var x = s.ship.x;

        // FIXME: do not hardcode with numbers, use keys instead
        // FIXME: make sure that the x coordinate is accurate when scaffolding or shuffled in the play
        for (int i = 0 ; i < ship.parts.Count; i++)
        {
            var part = ship.parts[i];
            switch (i)
            {
            case 0:
                if (x > 8)
                {
                    part.active = false;
                }
                else 
                {
                    part.active = true;
                }
                break;
            case 1:
                if (x > 7)
                {
                    part.active = false;
                }
                else 
                {
                    part.active = true;
                }
                break;
            case 3:
                if (x < 7)
                {
                    part.active = false;
                }
                else 
                {
                    part.active = true;
                }
                break;
            case 4:
                if (x < 6)
                {
                    part.active = false;
                }
                else 
                {
                    part.active = true;
                }
                break;
            }

        }
    }
}