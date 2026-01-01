using System.Diagnostics.CodeAnalysis;
using Nickel;

namespace Teuria.StarcourseDock;

internal static class CardPolarityExtension
{
    extension(Card card)
    {
        public bool IsOrange
        {
            get => ModEntry.Instance.Helper.ModData.GetModData<bool>(card, "polarity.orange");
            set => ModEntry.Instance.Helper.ModData.SetModData(card, "polarity.orange", value);
        }

        public bool IsBlue
        {
            get => !ModEntry.Instance.Helper.ModData.GetModData<bool>(card, "polarity.orange");
            set => ModEntry.Instance.Helper.ModData.SetModData(card, "polarity.orange", !value);
        }

        public Card? LinkedCard
        {
            get => ModEntry.Instance.Helper.ModData.GetModDataOrDefault<Card>(card, "polarity.card.linked");
            set => ModEntry.Instance.Helper.ModData.SetOptionalModData(card, "polarity.card.linked", value);
        }

        public bool HasLinkedCard => ModEntry.Instance.Helper.ModData.ContainsModData(card, "polarity.card.linked");

        public bool TryGetLinkedCard([NotNullWhen(true)] out Card? c)
        {
            ModEntry.Instance.Helper.ModData.TryGetModData(card, "polarity.card.linked", out Card? linkedCard);
            c = linkedCard;
            return linkedCard is not null;
        }
    }
}