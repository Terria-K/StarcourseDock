using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal static partial class AAttack_Global_Patches
{
    public static void Patch(IHarmony harmony)
    {
        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            prefix: new HarmonyMethod(GetGlobalState_Prefix),
            postfix: new HarmonyMethod(GetGlobalState_Postfix)
        );
    }

    public static AAttack? Global_AAttack;
    public static State? Global_State;

    private static void GetGlobalState_Prefix(State s, AAttack __instance)
    {
        Global_AAttack = __instance;
        Global_State = s;
    }

    private static void GetGlobalState_Postfix(State s, AAttack __instance)
    {
        Global_AAttack = null;
        Global_State = null;
    }
}
