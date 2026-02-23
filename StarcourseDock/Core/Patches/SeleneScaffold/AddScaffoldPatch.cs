using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AddScaffoldPatch 
{
    [HarmonyPatch(typeof(Events), nameof(Events.AddScaffold))]
    [HarmonyTranspiler]
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
}