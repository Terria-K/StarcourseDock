using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class ADrawCardSelective : CardAction
{
    public int index;

    public override void Begin(G g, State s, Combat c)
    {
        timer *= 0.5;

        c.DrawCardIdx(s, index);
    }
}
