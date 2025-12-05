namespace Teuria.StarcourseDock;

public sealed class WrathHurt : CardAction
{
    public int hurtAmount;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        c.QueueImmediate(new AHurt() { hurtShieldsFirst = true, hurtAmount = hurtAmount, targetPlayer = true });
    }
}