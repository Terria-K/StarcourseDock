using System.Reflection;
using System.Runtime.InteropServices;
using CutebaltCore;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoUpgradeCardPatches : IPatchable, IManualPatchable
{
    public static void ManualPatch(IHarmony harmony)
    {
        MethodInfo? cardUpgrade_Render_info = null!;

        foreach (var method in typeof(CardUpgrade).GetMethods())
        {
            if (method.Name.Contains("<Render>"))
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var p = parameters[0];
                    if (p.ParameterType == typeof(Upgrade))
                    {
                        cardUpgrade_Render_info = method;
                    }
                }
            }
        }

        ModEntry.Instance.Harmony.Patch(
            cardUpgrade_Render_info,
            postfix: new HarmonyMethod(CardUpgrade_Render_14_1_Postfix)
        );
    }

    private static G? global;

    [OnPrefix<CardUpgrade>(nameof(CardUpgrade.Render))]
    private static void CardUpgrade_Render_Prefix(G g)
    {
        global = g;
    }

    private static void CardUpgrade_Render_14_1_Postfix(Upgrade upgradePath, in Card __result)
    {
        if (ModEntry.Instance.Helper.ModData.TryGetModData(__result, "polarity.card.linked", out Card? linkedCard) && linkedCard is not null)
        {
            bool validUpgrade = false;
            Card card = Mutil.DeepCopy(linkedCard);
            var meta = card.GetMeta().upgradesTo.AsSpan();
            for (int i = 0; i < meta.Length; i++)
            {
                if (meta[i] == upgradePath)
                {
                    validUpgrade = true;
                    break;
                }
            }

            if (validUpgrade)
            {
                card.upgrade = upgradePath;
            }

            ModEntry.Instance.Helper.ModData.SetModData(__result, "polarity.card.linked", card);
        }
    }

    [OnPostfix<AUpgradeCardRandom>(nameof(AUpgradeCardRandom.BeginWithRoute))]
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

            if (ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkedCard) && linkedCard is not null)
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
