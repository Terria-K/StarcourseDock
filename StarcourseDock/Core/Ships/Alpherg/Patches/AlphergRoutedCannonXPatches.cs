using System.Reflection;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlphergRoutedCannonXPatches
{
    public static MethodBase TargetMethod()
    {
        MethodInfo? info = null!;

        foreach (var nestedType in typeof(AAttack).GetNestedTypes())
        {
            foreach (var method in nestedType.GetMethods())
            {
                if (method.Name.Contains("<GetFromX>"))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var p = parameters[0];
                        if (p.ParameterType == typeof(Part))
                        {
                            info = method;
                        }
                    }
                }
            }
        }

        return info;
    }

    internal static bool Prefix(Part p, ref bool __result)
    {
        var state = AAttack_Global_Patches.Global_State;
        var aAttack = AAttack_Global_Patches.Global_AAttack;
        if (state is null || aAttack is null || aAttack.targetPlayer)
        {
            return true;
        }

        var routedCannon = state.GetArtifactFromColorless<RoutedCannon>();

        if (routedCannon is not null && !routedCannon.disabled)
        {
            __result = (p.type == PType.empty || p.type == PType.cannon) && p.active;
            return false;
        }

        return true;
    }
}