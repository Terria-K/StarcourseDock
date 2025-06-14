using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class AlphergPisciumPatches : IPatchable
{
    [OnTranspiler<AAttack>(nameof(AAttack.Begin))]
    internal static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext([ILMatch.Newobj<AVolleyAttackFromAllCannons>()])
            .GotoPrev()
            .Emits([new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_2)])
            .EmitDelegate(
                (AAttack __instance, State s) =>
                {
                    ModEntry.Instance.Helper.ModData.SetModData(__instance, "piscium.volley", true);
                    Piscium.aAttack = __instance;
                }
            )
            .Generate();
    }

    [OnPrefix<AVolleyAttackFromAllCannons>(nameof(AVolleyAttackFromAllCannons.Begin))]
    public static void AVolleyAttackFromAllCannons_Begin_Prefix(
        AVolleyAttackFromAllCannons __instance,
        State s,
        Combat c
    )
    {
        if (
            ModEntry.Instance.Helper.ModData.TryGetModData(
                __instance.attack,
                "piscium.volley",
                out bool data
            )
        )
        {
            if (data)
            {
                var piscium = s.GetArtifactFromColorless<Piscium>();
                if (piscium is null)
                {
                    return;
                }

                piscium.isRight = !piscium.isRight;
                c.QueueImmediate(new ASwapScaffold() { isRight = piscium.isRight });
            }
        }
    }
}
