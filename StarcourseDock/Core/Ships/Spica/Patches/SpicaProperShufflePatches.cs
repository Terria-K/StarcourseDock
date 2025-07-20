using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class SpicaProperShufflePatches : IPatchable
{
    [OnTranspiler<AShuffleShip>(nameof(AShuffleShip.Begin))]
    private static IEnumerable<CodeInstruction> AShuffleShip_Begin_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(MoveType.After, ILMatch.Call("ToList"));
        cursor.Emits
        (
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2)
            ]
        );
        cursor.EmitDelegate((List<Part> p, AShuffleShip __instance, State s) =>
        {
            if (!__instance.targetPlayer)
            {
                return p;
            }
            if (s.ship.key == SpicaShip.SpicaEntry.UniqueName)
            {
                Part? rightWing = p.GetPartByKey("rightwing");
                Part? leftWing = p.GetPartByKey("leftwing");

                if (rightWing is null || leftWing is null)
                {
                    return p;
                }
                p.Remove(rightWing);
                p.Remove(leftWing);
                p.Insert(0, leftWing);
                p.Insert(p.Count, rightWing);
            }
            return p;
        });
        

        return cursor.Generate();
    }
}