using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class IoCardTableFlip
{
    [HarmonyPatch(typeof(Card), nameof(Card.GetDataWithOverrides))]
    [HarmonyPrefix]
    private static void Card_GetDataWithOverrides_Prefix(Card __instance, State state)
    {
        if (__instance is IAmFloppableThreeTimesAndFlippable flippable)
        {
            flippable.FlipOverrides = state.ship.Get(Status.tableFlip) > 0;
        }
    }
}

