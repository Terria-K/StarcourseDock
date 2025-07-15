using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;

namespace Teuria.StarcourseDock;

internal sealed partial class GlieseGlowPatches
{
    [OnTranspiler<Ship>(nameof(Ship.DrawTopLayer))]
    private static IEnumerable<CodeInstruction> Ship_DrawTopLayer_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext(
                [
                    ILMatch.Stloc(),
                    ILMatch.LdlocS(),
                    ILMatch.Ldfld("skin"),
                    ILMatch.Ldstr("cockpit_cicada"),
                ]
            )
            .ExtractOperand(0, out object? glowPos)
            .ExtractOperand(1, out object? part)
            .ExtractOperand(2, out object? skin)
            .GotoNext()
            .Emits(
                [
                    new CodeInstruction(OpCodes.Ldloc, glowPos),
                    new CodeInstruction(OpCodes.Ldloc, part),
                    new CodeInstruction(OpCodes.Ldfld, skin),
                    new CodeInstruction(OpCodes.Ldarg_1),
                ]
            )
            .EmitDelegate(
                (Vec glowPos, string skin, G g) =>
                {
                    if (skin == GlieseShip.GlieseCockpit.UniqueName)
                    {
                        Glow.Draw(
                            glowPos + new Vec(2.0, 24.0),
                            80.0,
                            new Color(0.0, 0.5, 1.0, 1.0).gain(
                                0.9 + Math.Sin(g.state.time * 8.0) * 0.1
                            )
                        );
                    }
                }
            )
            .Generate();
    }
}
