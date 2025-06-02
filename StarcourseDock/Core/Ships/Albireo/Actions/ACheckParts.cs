namespace Teuria.StarcourseDock;

internal sealed class ACheckParts : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        var ship = s.ship;
        var x = s.ship.x;

        for (int i = 0; i < ship.parts.Count; i++)
        {
            var part = ship.parts[i];
            int realX = x - i;

            switch (part.key)
            {
                case "left_missile":
                case "left_cannon":
                    part.active = !(realX > 10);
                    break;
                case "right_cannon":
                case "right_missile":
                    part.active = !(realX < 0);
                    break;
            }
        }
    }
}
