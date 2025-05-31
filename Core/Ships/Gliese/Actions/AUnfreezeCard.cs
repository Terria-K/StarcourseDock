namespace Teuria.StarcourseDock;

internal class AUnfreezeCard : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        foreach (var card in c.hand)
        {
            if (
                ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(
                    s,
                    card,
                    CrystalCore.FrozenTrait
                )
            )
            {
                ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                    s,
                    card,
                    CrystalCore.FrozenTrait,
                    null,
                    true
                );

                card.unplayableOverride = false;
            }
        }
    }
}
