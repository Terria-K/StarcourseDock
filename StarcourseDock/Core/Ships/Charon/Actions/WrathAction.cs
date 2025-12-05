namespace Teuria.StarcourseDock;

public sealed class WrathAction : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        c.QueueImmediate(new AAttack() { damage = Card.GetActualDamage(s, 1)});
    }
}
