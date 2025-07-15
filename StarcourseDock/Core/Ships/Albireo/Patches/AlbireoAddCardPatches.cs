using CutebaltCore;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoAddCardPatches : IPatchable
{
    [OnPrefix<AAddCard>(nameof(AAddCard.Begin))]
    private static void AAddCard_Begin_Prefix(State s, AAddCard __instance)
    {
        var doubleDeck = s.GetArtifactFromColorless<DoubleDeck>();
        if (s.route is Combat || doubleDeck is null)
        {
            return;
        }
        var card = __instance.card;

        if (card.GetMeta().deck == Deck.trash)
        {
            return;
        }

        Card linkCard = AlbireoCardUtilities.GetRandomCard(card, s, card.temporaryOverride ?? false || card.GetData(s).temporary, true, 0);

        bool validUpgrade = false;
        var upgradePath = card.upgrade;
        var meta = linkCard.GetMeta().upgradesTo.AsSpan();
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
            linkCard.upgrade = upgradePath;
        }

        ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
            s, card, AlbireoKit.PolarityTrait, true, true
        );

        ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(
            s, linkCard, AlbireoKit.PolarityTrait, true, true
        );

        ModEntry.Instance.Helper.ModData.SetOptionalModData(card, "polarity.card.linked", linkCard);
        ModEntry.Instance.Helper.ModData.SetModData(card, "polarity.orange", doubleDeck.isOrange);
        ModEntry.Instance.Helper.ModData.SetModData(linkCard, "polarity.orange", !doubleDeck.isOrange);
    }
}
