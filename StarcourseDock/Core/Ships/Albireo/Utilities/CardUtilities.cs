namespace Teuria.StarcourseDock;

internal static class AlbireoCardUtilities
{
    public static List<Card> GetRandomCards(State s, bool temp, bool inCombat, int discount)
    {
        return CardReward.GetOffering(s, 1, null, BattleType.Normal, null, null, temp, inCombat, discount, false);
    }

    public static Card GetRandomCard(Card card, State s, bool temp, bool inCombat, int discount)
    {
        return GetValidRandomCard(card, s, 1, null, BattleType.Normal, null, null, temp, inCombat, discount, false);
    }

    public static Card GetValidRandomCard(Card card, State s, int count, Deck? limitDeck = null, BattleType battleType = BattleType.Normal, Rarity? rarityOverride = null, bool? overrideUpgradeChances = null, bool makeAllCardsTemporary = false, bool inCombat = false, int discount = 0, bool isEvent = false)
    {
        Card linkCard;
        bool isExe;
        do
        {
            linkCard = CardReward.GetOffering(s, 1, limitDeck, battleType, rarityOverride, overrideUpgradeChances, makeAllCardsTemporary, inCombat, discount, isEvent)[0];
            isExe = ModEntry.Instance.EssentialAPI.IsExeCardType(linkCard.GetType());
        }
        // retries if its the same card, exe (if not exe), or not exe (if exe)
        while (
            linkCard.Key() == card.Key() // same
            || (isExe && !ModEntry.Instance.EssentialAPI.IsExeCardType(card.GetType()))
        );

        return linkCard;
    }
}