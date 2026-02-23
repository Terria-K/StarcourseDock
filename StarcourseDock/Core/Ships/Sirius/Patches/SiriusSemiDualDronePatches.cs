using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class SiriusSemiDualDronePatches
{
    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyPostfix]
    private static void AAttack_Begin_Postfix(AAttack __instance, Combat c)
    {
        if (__instance.fromDroneX != null)
        {
            if (
                c.stuff.TryGetValue(__instance.fromDroneX.Value, out var drone)
                && drone is SiriusSemiDualDrone
            )
            {
                drone.pulse = 1.0;
            }
        }
    }

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext(
                MoveType.After,
                [
                    ILMatch.Call("DeepCopy"),
                    ILMatch.Stfld("attackCopy"),
                    ILMatch.Callvirt("QueueImmediate"),
                ]
            )
            .Emits([new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_3)])
            .EmitDelegate(
                (AAttack __instance, Combat combat) =>
                {
                    combat.QueueImmediate(
                        new ASiriusInquisitorShoot()
                        {
                            attackCopy = Mutil.DeepCopy<AAttack>(__instance),
                        }
                    );
                }
            )
            .Generate();
    }

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.GetTooltips))]
    [HarmonyPrefix]
    private static void AAttack_GetTooltips_Prefix(State s)
    {
        Combat? c = s.route as Combat;
        if (c == null)
        {
            return;
        }

        foreach (StuffBase thing in c.stuff.Values)
        {
            if (thing is not SiriusSemiDualDrone)
            {
                continue;
            }
            thing.hilight = 2;
        }
    }

    [HarmonyPatch(typeof(AAttack), nameof(AAttack.DoWeHaveCannonsThough))]
    [HarmonyPrefix]
    private static bool AAttack_DoWeHaveCannonsThough_Prefix(State s, ref bool __result)
    {
        if (s.route is not Combat c)
        {
            return true;
        }

        foreach (var (x, stuff) in c.stuff)
        {
            if (stuff is SiriusSemiDualDrone dualDrone && !dualDrone.targetPlayer)
            {
                __result = true;
                return false;
            }
        }

        return true;
    }
}
