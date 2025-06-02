namespace Teuria.StarcourseDock;

internal sealed class ADoubler : CardAction
{
    public Card? card;

    public override void Begin(G g, State s, Combat c)
    {
        if (card is null)
        {
            return;
        }
        var actions = new List<CardAction>();
        timer = 0.0;

        List<CardAction> toAdd = [.. card.GetActionsOverridden(s, c).Where(a => a is not AEndTurn)];

        bool isSpawning = toAdd.Where(x => x is ASpawn).Any();
        if (isSpawning)
        {
            toAdd.Add(new ADroneMove { dir = 1 });
            c.Queue(new ADroneMove { dir = -1 });
        }
        actions.InsertRange(0, toAdd);
        foreach (CardAction action in actions)
        {
            c.Queue(action);
        }
    }
}
