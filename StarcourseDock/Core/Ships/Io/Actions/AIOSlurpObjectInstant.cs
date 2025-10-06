namespace Teuria.StarcourseDock;

internal sealed class AIOSlurpObjectInstant : CardAction
{
    public required StuffBase thing;
    public int dist;

    public override void Begin(G g, State s, Combat c)
    {
        thing = Mutil.DeepCopy(thing);
        thing.targetPlayer = true;

        c.QueueImmediate(new AAddCard
        {
            card = new PulledAndRelease
            {
                thing = thing,
                isLeft = dist == -1,
                isCentered = dist == 0
            },
            destination = CardDestination.Hand
        });
    }
}
