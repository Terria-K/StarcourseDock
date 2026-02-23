using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class SpicaTriscaffoldingPatches
{
    [HarmonyPatch(typeof(TridimensionalCockpit), nameof(TridimensionalCockpit.OnReceiveArtifact))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnReceiveArtifact_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext([ILMatch.Stfld("skin")])
            .Emits([new CodeInstruction(OpCodes.Ldarg_1)])
            .EmitDelegate(
                (string skin, State s) =>
                {
                    if (s.ship.key != SpicaShip.SpicaEntry.UniqueName)
                    {
                        return skin;
                    }

                    return SpicaShip.SpicaTriScaffold.UniqueName;
                }
            )
            .Generate();
    }
}
