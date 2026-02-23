using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlphergPisciumPatches
{
    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyPrefix]
    private static void AAttack_Begin_Prefix(AAttack __instance)
    {
        Piscium.aAttack = __instance;
    } 

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyFinalizer]
    private static void AAttack_Begin_Finalizer()
    {
        Piscium.aAttack = null;
    } 
}
