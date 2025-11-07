namespace Teuria.StarcourseDock;

internal sealed class AIOSlurpObjectInstant : CardAction
{
    public required StuffBase thing;
    public int dist;
    public bool aUpgrade;

    public override void Begin(G g, State s, Combat c)
    {
        thing = Mutil.DeepCopy(thing);
        thing.targetPlayer = true;

        c.QueueImmediate(new AAddCard
        {
            card = new Release
            {
                thing = thing,
                isLeft = dist == -1,
                isCentered = dist == 0,
                upgrade = aUpgrade ? Upgrade.A : Upgrade.None
            },
            destination = CardDestination.Hand
        });
    }
}
