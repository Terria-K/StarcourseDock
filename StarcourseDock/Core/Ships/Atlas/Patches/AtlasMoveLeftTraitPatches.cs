using System.Reflection.Emit;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AtlasMoveLeftTraitPatches
{
    [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
    [HarmonyPrefix]
    private static void Ship_NormalDamage_Prefix(Ship __instance, Combat c, int? maybeWorldGridX)
    {
        if (maybeWorldGridX is int x && 
            __instance.GetPartAtWorldX(x) is Part part && 
            part.stunModifier == AtlasShip.LeftMoveStunModifier)
        {
            c.QueueImmediate(new AMove()
            {
                dir = -3,
                targetPlayer = true
            });
        }
    }

    [HarmonyPatch(typeof(Ship), nameof(Ship.RenderPartUI))]
    [HarmonyPostfix]
    private static void Ship_RenderPartUI_Postfix(
        Ship __instance,
        G g,
        Part part,
        int localX,
        string keyPrefix,
        bool isPreview
    )
    {
        if (part.invincible || part.stunModifier != AtlasShip.LeftMoveStunModifier)
        {
            return;
        }

        if (
            g.boxes.FirstOrDefault(b => b.key == new UIKey(StableUK.part, localX, keyPrefix))
            is not { } box
        )
        {
            return;
        }

        var offset = isPreview ? 25 : 34;
        var v = box.rect.xy + new Vec(0, __instance.isPlayerShip ? (offset - 16) : 8);

        var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
        Draw.Sprite(Sprites.icons_moveLeftTrait.Sprite, v.x + 9, v.y, color: color);
    }

    [HarmonyPatch(typeof(Ship), nameof(Ship.RenderPartUI))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Ship_RenderPartUI_Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new ILCursor(generator, instructions)
            .GotoNext(MoveType.Before, [ILMatch.LdcI4(0), ILMatch.Callvirt(), ILMatch.Stloc()])
            .ExtractOperand(2, out object? bb)
            .GotoNext(
                MoveType.After,
                [
                    ILMatch.Ldarg(1),
                    ILMatch.Ldfld("tutorial"),
                    ILMatch.Ldarg(1),
                    ILMatch.Ldarg(0),
                    ILMatch.Ldarg(3),
                    ILMatch.LdlocS(),
                    ILMatch.Callvirt(),
                ]
            )
            .Emits(
                [
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldloc_S, bb),
                ]
            )
            .EmitDelegate(
                (G g, Part part, Box bb) =>
                {
                    if (!bb.IsHover())
                    {
                        return;
                    }

                    Vec ttPos = bb.rect.xy + new Vec(16.0, 0.0);

                    if (part.stunModifier == AtlasShip.LeftMoveStunModifier)
                    {
                        g.tooltips.Add(
                            ttPos,
                            new GlossaryTooltip(
                                $"{ModEntry.Instance.Package.Manifest.UniqueName}::LeftMoveStunModifier"
                            )
                            {
                                Title = Localization.Str_ship_Atlas_parttrait_LeftMoveStun_name(),
                                TitleColor = Colors.parttrait,
                                Description = Localization.Str_ship_Atlas_parttrait_LeftMoveStun_description(),
                                Icon = Sprites.icons_moveLeftTrait.Sprite,
                            }
                        );

                        g.tooltips.Add(
                            ttPos,
                            new TTGlossary("action.moveLeft", $"<c=boldPink>3</c>")
                        );
                    }
                }
            )
            .Generate();
    }
}