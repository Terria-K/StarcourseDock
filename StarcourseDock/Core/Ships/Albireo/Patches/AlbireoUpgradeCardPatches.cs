using System.Runtime.InteropServices;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlbireoUpgradeCardPatches
{
    private static G? global;

    [HarmonyPatch(typeof(CardUpgrade), nameof(CardUpgrade.Render))]
    [HarmonyPrefix]
    private static void CardUpgrade_Render_Prefix(G g)
    {
        global = g;
    }

    [HarmonyPatch(typeof(AUpgradeCardRandom), nameof(AUpgradeCardRandom.BeginWithRoute))]
    [HarmonyPostfix]
    private static void AUpgradeCardRandom_BeginWithRoute_Postfix(State s, in Route __result)
    {
        if (__result is not ShowCards showCards)
        {
            return;
        }

        var cardIds = CollectionsMarshal.AsSpan(showCards.cardIds);

        for (int i = 0; i < cardIds.Length; i += 1)
        {
            int id = cardIds[i];
            var deck = CollectionsMarshal.AsSpan(s.deck);

            Card? card = null;

            for (int j = 0; j < deck.Length; j += 1)
            {
                card = deck[j];

                if (card.uuid == id)
                {
                    break;
                }
            }

            if (card is null)
            {
                continue;
            }

            if (card.TryGetLinkedCard(out Card? linkedCard))
            {
                var upgradePath = card.upgrade;
                var availableUpgrades = linkedCard.GetMeta().upgradesTo.AsSpan();

                bool upgradeAvailable = false;

                for (int u = 0; u < availableUpgrades.Length; u += 1)
                {
                    if (availableUpgrades[u] == upgradePath)
                    {
                        upgradeAvailable = true;
                        break;
                    }
                }

                if (upgradeAvailable)
                {
                    linkedCard.upgrade = upgradePath;
                }
            }
        }
    }
}
