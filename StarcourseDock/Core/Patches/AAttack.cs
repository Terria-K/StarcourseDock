using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal partial class AAttack_Global_Patches
{
    public static AAttack? Global_AAttack;
    public static State? Global_State;

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyPrefix]
    private static void GetGlobalState_Prefix(State s, AAttack __instance)
    {
        Global_AAttack = __instance;
        Global_State = s;
    }

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyPostfix]
    private static void GetGlobalState_Postfix()
    {
        Global_AAttack = null;
        Global_State = null;
    }
}
