using System.Runtime.InteropServices;

namespace Teuria.StarcourseDock;

internal sealed class APolaritySwitch : CardAction
{
    public bool isOrange;
    public override void Begin(G g, State s, Combat c)
    {
        var parts = CollectionsMarshal.AsSpan(s.ship.parts);

        if (isOrange)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                switch (p.key)
                {
                    case "ab_missiles":
                        p.skin = AlbireoShip.AlbireoMissileBayOrange.UniqueName;
                        break;
                    case "ab_cannon":
                        p.skin = AlbireoShip.AlbireoCannonOrange.UniqueName;
                        break;
                    case "ab_cockpit":
                        p.skin = AlbireoShip.AlbireoCockpitOrange.UniqueName;
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                switch (p.key)
                {
                    case "ab_missiles":
                        p.skin = AlbireoShip.AlbireoMissileBayBlue.UniqueName;
                        break;
                    case "ab_cannon":
                        p.skin = AlbireoShip.AlbireoCannonBlue.UniqueName;
                        break;
                    case "ab_cockpit":
                        p.skin = AlbireoShip.AlbireoCockpitBlue.UniqueName;
                        break;
                }
            }
        }

        List<Card> cardChanges = new List<Card>(s.deck.Count);
        var oldDeck = s.deck;

        ChangeCardList(oldDeck, true);

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

        void ChangeCardList(List<Card> deck, bool isDeck = false)
        {
            foreach (var card in deck)
            {
                if (card.TryGetLinkedCard(out Card? linkCard))
                {
                    linkCard.flipAnim = 1.0;
                    linkCard.flopAnim = -1.0;
                    linkCard.waitBeforeMoving = 0.0;
                    linkCard.pos = card.pos;
                    linkCard.drawAnim = 0.0;
                    cardChanges.Add(linkCard);

                    card.LinkedCard = null;
                    linkCard.LinkedCard = card;

                    ModEntry.Instance.CombatQolAPI?.MarkCardAsOkayToBeGone(c, card);
                    continue;
                }

                cardChanges.Add(card);
            }
        }
    }
}
