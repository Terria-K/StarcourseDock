using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class GlieseFrozenCardRenderPatches 
{
    [OnTranspiler<Card>(nameof(Card.Render))]
    private static IEnumerable<CodeInstruction> Render(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext(MoveType.After, [ILMatch.LdlocS(), ILMatch.Callvirt("ExtraRender")])
            .ExtractOperand(0, out object? v)
            .Emits(
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldloc, v),
                    new CodeInstruction(OpCodes.Ldarg_3),
                ]
            )
            .EmitDelegate(
                (Card card, G g, Vec v, State s) =>
                {
                    if (
                        !ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(
                            s,
                            card,
                            GlieseKit.FrozenTrait
                        )
                        || !ModEntry.Instance.Helper.ModData.TryGetModData(
                            card,
                            "FrozenCount",
                            out int count
                        )
                        || count <= 3
                    )
                    {
                        return;
                    }
                    Draw.Sprite(
                        Sprites.cardShared_layer_border_frozen_card.Sprite,
                        v.x - 3,
                        v.y - 3,
                        color: Colors.white.fadeAlpha(0.7)
                    );
                }
            )
            .Generate();
    }
}
