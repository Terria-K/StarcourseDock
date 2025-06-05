using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class SpicaFixedStarPatches : IPatchable
{
    [OnPrefix<AMove>(nameof(AMove.Begin))]
    private static bool AMove_Begin_Prefix(AMove __instance, State s, Combat c)
    {
        if (
            s.HasArtifactFromColorless<FixedStar>()
            && __instance.targetPlayer
            && __instance.fromEvade
        )
        {
            __instance.timer = 0f;
            c.QueueImmediate(
                new ACannonMove()
                {
                    dir = __instance.dir,
                    ignoreHermes = __instance.ignoreHermes,
                    preferRightWhenZero = __instance.preferRightWhenZero,
                }
            );
            return false;
        }

        return true;
    }
}

internal sealed partial class SpicaTriscaffoldingPatches : IPatchable
{
    [OnTranspiler<TridimensionalCockpit>(nameof(TridimensionalCockpit.OnReceiveArtifact))]
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
