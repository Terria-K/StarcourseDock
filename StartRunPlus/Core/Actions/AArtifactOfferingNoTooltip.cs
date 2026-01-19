namespace Teuria.StartRunPlus;

internal sealed class AArtifactOfferingNoTooltip : CardAction
{
    public int amount = 3;
    public bool canSkip = true;
    public Deck? limitDeck;
    public List<ArtifactPool>? limitPools;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        timer = 0.0;
        var offering = new AArtifactOffering()
        {
            amount = amount,
            canSkip = canSkip,
            limitDeck = limitDeck,
            limitPools = limitPools
        };

        return offering.BeginWithRoute(g, s, c);
    }
}