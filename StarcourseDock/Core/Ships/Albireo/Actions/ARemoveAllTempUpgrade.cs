namespace Teuria.StarcourseDock;

internal sealed class ARemoveAllTempUpgrade : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        // for Temp Upgrades
        foreach (var card in s.deck)
        {
            if (!ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkCard) || linkCard is null)
            {
                continue;
            }

            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
                s,
                card,
                ModEntry.Instance.KokoroAPI.V2.TemporaryUpgrades.CardTrait,
                false,
                true
            );
        }
    }
}
