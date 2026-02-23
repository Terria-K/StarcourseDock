using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlbireoStateAllowModifyPolarDeckPatches
{
    [HarmonyPatch(typeof(State), nameof(State.EndRun))]
    [HarmonyPrefix]
    private static void State_EndRun_Prefix(State __instance)
    {
        if (__instance.route is not Combat)
        {
            var oldDeck = __instance.deck;

            ExtractPolarCardToDeck(oldDeck);
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.ReturnCardsToDeck))]
    [HarmonyPrefix]
    private static void Combat_ReturnCardsToDeck_Prefix(Combat __instance, State state)
    {
        if (!state.HasArtifact<DoubleDeck>())
        {
            return;
        }

        var oldDeck = state.deck;

        ExtractPolarCardToDeck(oldDeck);

        var oldHand = __instance.hand;

        ExtractPolarCardToDeck(oldHand);

        var oldExhaust = __instance.exhausted;

        ExtractPolarCardToDeck(oldExhaust);

        var oldDiscard = __instance.discard;

        ExtractPolarCardToDeck(oldDiscard);


        state.rewardsQueue.QueueImmediate(new ARemoveCardByPolarity());
    }

    private static void ExtractPolarCardToDeck(in List<Card> deck)
    {
        List<Card> addToDeck = [];
        foreach (var card in deck)
        {
            if (card.TryGetLinkedCard(out Card? linkCard))
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
}