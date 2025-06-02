using CutebaltCore;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal partial class AAttack_Global_Patches : IPatchable
{
    public static AAttack? Global_AAttack;
    public static State? Global_State;

    [OnPrefix<AAttack>("Begin")]
    private static void GetGlobalState_Prefix(State s, AAttack __instance)
    {
        Global_AAttack = __instance;
        Global_State = s;
    }

    [OnPostfix<AAttack>("Begin")]
    private static void GetGlobalState_Postfix(State s, AAttack __instance)
    {
        Global_AAttack = null;
        Global_State = null;
    }
}
