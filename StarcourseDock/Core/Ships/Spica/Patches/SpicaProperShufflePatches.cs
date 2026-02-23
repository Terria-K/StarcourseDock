using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class SpicaProperShufflePatches
{
    [HarmonyPatch(typeof(AShuffleShip), nameof(AShuffleShip.Begin))]
    [HarmonyTranspiler]
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
                Part? rightWing = p.GetPartByKey("Starcourse::rightwing");
                Part? leftWing = p.GetPartByKey("Starcourse::leftwing");

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