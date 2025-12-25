using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CutebaltCore;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed partial class AddScaffoldPatch : IPatchable, IManualPatchable
{
    public static IEnumerable<CodeInstruction> Events_AddScaffold_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(MoveType.After, [
            ILMatch.Ldstr("scaffolding"), 
            ILMatch.Stfld("skin"),
            ILMatch.Stloc().TryGetLocalIndex(out BoxReference<int> op)
        ]);

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldloc, op.Value);
        cursor.EmitDelegate((State s, Part part) =>
        {
            ref var scaff = ref CollectionsMarshal.GetValueRefOrNullRef(SeleneScaffoldManager.Scaffolds, s.ship.key);

            if (Unsafe.IsNullRef(ref scaff))
            {
                return;
            }

            part.skin = scaff.UniqueName;
        });

        return cursor.Generate();
    }

    public static void ManualPatch(IHarmony harmony)
    {
        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Events), nameof(Events.AddScaffold)),
            transpiler: new HarmonyMethod(Events_AddScaffold_Transpiler)
        );
    }
}