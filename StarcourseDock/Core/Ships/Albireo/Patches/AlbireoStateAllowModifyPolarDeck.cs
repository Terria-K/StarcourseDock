using CutebaltCore;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoStateAllowModifyPolarDeck : IPatchable
{
    [OnPrefix<Combat>(nameof(Combat.ReturnCardsToDeck))]
    private static void Combat_ReturnCardsToDeck_Prefix(Combat __instance, State state)
    {
        if (!state.HasArtifact<DoubleDeck>())
        {
            return;
        }

        var oldHand = __instance.hand;

        PutThingsBack(oldHand);

        var oldExhaust = __instance.exhausted;

        PutThingsBack(oldExhaust);

        var oldDiscard = __instance.discard;

        PutThingsBack(oldDiscard);

        static void PutThingsBack(in List<Card> deck)
        {
            List<Card> addToDeck = [];
            foreach (var card in deck)
            {
                if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkCard) && linkCard is not null)
                {
                    addToDeck.Add(linkCard);
                    ModEntry.Instance.Helper.ModData.SetOptionalModData<bool>(linkCard, "polarity.card.toRemove", true);
                }
            }

            foreach (var a in addToDeck)
            {
                deck.Add(a);
            }
        }
        state.rewardsQueue.QueueImmediate(new ARemoveCardByPolarity());
    }
}