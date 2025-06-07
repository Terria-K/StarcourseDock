namespace Teuria.StarcourseDock;

internal class AUnfreezeCard : CardAction
{
    public bool shouldDraw;

    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        var card = this.selectedCard;

        if (card is null)
        {
            return;
        }

        if (
            ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card, GlieseKit.FrozenTrait)
        )
        {
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                s,
                card,
                GlieseKit.FrozenTrait,
                false,
                false
            );

            card.unplayableOverride = false;
            ModEntry.Instance.Helper.ModData.SetModData(card, "FrozenCount", 0);
        }

        if (shouldDraw)
        {
            c.QueueImmediate(new ChooseCardToPutInHand() { selectedCard = card, timer = 0.0 });
        }
    }
}
