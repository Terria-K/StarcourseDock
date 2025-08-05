using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class SiriusMissileBayPatches : IPatchable
{
    [OnTranspiler<Card>(nameof(Card.RenderAction))]
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
            .EmitDelegate(RenderAction_Lambda)
            .Generate();
    }

    private static void RenderAction_Lambda(CardAction action, ref int e)
    {
        if (action is AToggleMissileBay or AActivateAllPartsWrapper)
        {
            e -= 9;
        }
    }
}
