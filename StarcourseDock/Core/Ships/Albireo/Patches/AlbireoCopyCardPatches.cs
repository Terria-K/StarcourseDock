using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlbireoCopyCardPatches
{
    [HarmonyPatch(typeof(Card), nameof(Card.CopyWithNewId))]
    [HarmonyPostfix]
    internal static void Card_CopyWithNewId_Postfix(Card __result)
    {
        if (__result.TryGetLinkedCard(out var linkedCard))
        {
            var newCard = linkedCard.CopyWithNewId();
            __result.LinkedCard = newCard;
        }
    }
}
