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
                if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkCard) && linkCard is not null)
                {
                    linkCard.flipAnim = 2.0;
                    cardChanges.Add(linkCard);
                    ModEntry.Instance.Helper.ModData.SetOptionalModData<Card>(card, "polarity.card.linked", null);
                    ModEntry.Instance.Helper.ModData.SetOptionalModData(linkCard, "polarity.card.linked", card);
                    continue;
                }

                cardChanges.Add(card);
            }
        }
    }
}