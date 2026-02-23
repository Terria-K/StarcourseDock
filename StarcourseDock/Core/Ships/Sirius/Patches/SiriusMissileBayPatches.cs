using System.Reflection.Emit;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class SiriusMissileBayPatches
{
    [HarmonyPatch(typeof(Card), nameof(Card.RenderAction))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Card_RenderAction_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext([ILMatch.LdlocaS(), ILMatch.Ldarg(3)])
            .ExtractOperand(0, out object? unknownType)
            .GotoNext([ILMatch.Ldloc(0), ILMatch.Ldfld("w")])
            .ExtractOperand(1, out object? fld)
            .Reset()
            .GotoNext([ILMatch.Ldloc(0), ILMatch.Ldfld("action")])
            .ExtractOperand(1, out object? action)
            .GotoNext(MoveType.After, [ILMatch.Isinst(), ILMatch.Stloc(3)])
            .Emits(
                [
                    new CodeInstruction(OpCodes.Ldloca, unknownType),
                    new CodeInstruction(OpCodes.Ldfld, action),
                    new CodeInstruction(OpCodes.Ldloca, unknownType),
                    new CodeInstruction(OpCodes.Ldflda, fld),
                ]
            )
            .EmitDelegate((CardAction action, ref int e) =>
            {
                if (action is AToggleMissileBay or AActivateAllPartsWrapper)
                {
                    e -= 9;
                }
            })
            .Generate();
    }
}
