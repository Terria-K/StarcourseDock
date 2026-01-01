using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class APolaritySwitch : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        List<Card> cardChanges = new List<Card>(s.deck.Count);
        var oldDeck = s.deck;

        ChangeCardList(oldDeck);

        s.deck = [.. cardChanges];

        cardChanges.Clear();

        if (c is null)
        {
            return;
        }

        var oldHand = c.hand;

        ChangeCardList(oldHand);

        c.hand = [.. cardChanges];

        cardChanges.Clear();

        var oldExhaust = c.exhausted;

        ChangeCardList(oldExhaust);

        c.exhausted = [.. cardChanges];

        cardChanges.Clear();

        var oldDiscard = c.discard;

        ChangeCardList(oldDiscard);

        c.discard = [.. cardChanges];

        cardChanges.Clear();

        void ChangeCardList(List<Card> deck)
        {
            foreach (var card in deck)
            {
                if (card.TryGetLinkedCard(out Card? linkCard))
                {
                    linkCard.flipAnim = 2.0;
                    linkCard.waitBeforeMoving = 0.0;
                    card.drawAnim = 0.0;
                    card.waitBeforeMoving = 0.0;
                    card.pos = card.targetPos;
                    linkCard.pos = linkCard.targetPos;
                    linkCard.drawAnim = 0.0;
                    cardChanges.Add(linkCard);

                    card.LinkedCard = null;
                    linkCard.LinkedCard = card;
                    continue;
                }

                cardChanges.Add(card);
            }
        }
    }
}
