using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoRunSummaryPatches : IPatchable
{
    [OnPrefix<RunSummary>(nameof(RunSummary.SaveFromState))]
    private static void RunSummary_SaveFromState_Prefix(State s)
    {
        List<Card> cloneDeck = [.. s.deck];

        foreach (var card in s.deck)
        {
            if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkCard) && linkCard is not null)
            {
                cloneDeck.Add(linkCard);
                continue;
            }
        }

        s.deck = cloneDeck;
    }
}
