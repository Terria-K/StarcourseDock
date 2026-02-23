using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlphergProperShufflePatches
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
            if (s.ship.key == AlphergShip.AlphergEntry.UniqueName)
            {
                int count = p.Count;

                int c = Mutil.Clamp(s.rngActions.NextInt() % count, 1, count - 2);

                Part? wingRight = p.GetPartByKey("Starcourse::wingright");

                if (wingRight is not null)
                {
                    p.Remove(wingRight);
                    p.Insert(c, wingRight);
                    return p;
                }

                Part? wingLeft = p.GetPartByKey("Starcourse::wingleft");

                if (wingLeft is not null)
                {
                    p.Remove(wingLeft);
                    p.Insert(c, wingLeft);
                    return p;
                }
            }
            return p;
        });
        

        return cursor.Generate();
    }
}
