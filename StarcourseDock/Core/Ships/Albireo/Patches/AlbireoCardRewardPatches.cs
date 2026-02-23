using System.Runtime.InteropServices;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlbireoCardRewardPatches
{
    [HarmonyPatch(typeof(ACardOffering), nameof(ACardOffering.BeginWithRoute))]
    [HarmonyPrefix]
    private static void ACardOffering_BeginWithRoute_Postfix(ACardOffering __instance, State s, in Route __result)
    {
        if (__result is not CardReward cardReward)
        {
            return;
        }

        var doubleStar = s.GetArtifactFromColorless<DoubleDeck>();
        if (doubleStar is null)
        {
            return;
        }

        var cardRewards = CollectionsMarshal.AsSpan(cardReward.cards);

        for (int i = 0; i < cardRewards.Length; i++)
        {
            var card = cardRewards[i];
            var linkCard = AlbireoCardUtilities.GetValidRandomCard(
                card, s, __instance.amount,
                __instance.limitDeck,
                __instance.battleType ?? BattleType.Normal,
                __instance.rarityOverride,
                null,
                __instance.makeAllCardsTemporary,
                __instance.inCombat,
                __instance.discount,
                __instance.isEvent);
            bool validUpgrade = false;

            var upgradePath = card.upgrade;
            var meta = linkCard.GetMeta().upgradesTo.AsSpan();
            for (int j = 0; j < meta.Length; j++)
            {
                if (meta[j] == upgradePath)
                {
                    validUpgrade = true;
                    break;
                }
            }

            if (validUpgrade)
            {
                linkCard.upgrade = upgradePath;
            }
            else
            {
                linkCard.upgrade = Upgrade.None;
            }

            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                s, card, AlbireoKit.PolarityTrait, true, true
            );

            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                s, linkCard, AlbireoKit.PolarityTrait, true, true
            );

            card.LinkedCard = linkCard;
            card.IsOrange = doubleStar.isOrange;
            linkCard.IsOrange = !doubleStar.isOrange;
        }

    }

    [HarmonyPatch(typeof(State), nameof(State.PopulateRun))]
    [HarmonyPriority(-400)]
    [HarmonyPostfix]
    private static void State_PopulateRun_Postfix(State __instance)
    {
        if (!__instance.HasArtifactFromColorless<DoubleDeck>())
        {
            return;
        }

        foreach (var card in __instance.deck)
        {
            if (card.GetMeta().deck == Deck.trash)
            {
                continue;
            }

            Deck? limitDeck = null;

            if (ModEntry.Instance.EssentialAPI.IsExeCardType(card.GetType()))
            {
                limitDeck = Deck.colorless;
            }

            Card linkCard = AlbireoCardUtilities.GetValidRandomCard(card, __instance, 1, limitDeck, BattleType.Normal);

            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                __instance, card, AlbireoKit.PolarityTrait, true, true
            );

            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                __instance, linkCard, AlbireoKit.PolarityTrait, true, true
            );

            card.LinkedCard = linkCard;
            card.IsOrange = false;
            linkCard.IsOrange = true;
        }
    }
}
