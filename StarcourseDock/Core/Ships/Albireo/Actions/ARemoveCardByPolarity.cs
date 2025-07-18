using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class ARemoveCardByPolarity : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        List<Card> r = [];
        foreach (var card in s.deck)
        {
            if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.toRemove", out bool? toRemove) && toRemove is not null)
            {
                ModEntry.Instance.Helper.ModData.SetOptionalModData<bool>(card, "polarity.card.toRemove", null);
                r.Add(card);
            }
        }

        foreach (var i in r)
        {
            s.deck.Remove(i);
        }
    }
}