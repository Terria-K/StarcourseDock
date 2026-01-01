using CutebaltCore;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoCopyCardPatches : IPatchable
{
    [OnPostfix<Card>(nameof(Card.CopyWithNewId))]
    internal static void Card_CopyWithNewId_Postfix(Card __result)
    {
        if (__result.TryGetLinkedCard(out var linkedCard))
        {
            var newCard = linkedCard.CopyWithNewId();
            __result.LinkedCard = newCard;
        }
    }
}
