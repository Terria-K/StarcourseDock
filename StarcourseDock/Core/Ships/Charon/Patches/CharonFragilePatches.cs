using System.Reflection.Emit;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal partial class CharonFragilePatches
{
    [HarmonyPatch(typeof(Ship), nameof(Ship.ModifyDamageDueToParts))]
    [HarmonyFinalizer]
    private static Exception? ModifyDamageDueToParts_Finalizer(Part part, int incomingDamage, ref int __result, Exception __exception)
    {
        PDamMod mod = part.GetDamageModifier();
        if (mod == CharonShip.Fragile)
        {
            if (!part.invincible)
            {
                __result = incomingDamage * 3;
            }
            else
            {
                __result = 0;
            }

            part.brittleIsHidden = false;
            return null;
        }

        return __exception;
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
        if (part.invincible || part.damageModifierOverrideWhileActive != CharonShip.Fragile)
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
        Draw.Sprite(Sprites.icons_fragile.Sprite, v.x + 1.0, v.y, color: color);
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

                    if (part.damageModifierOverrideWhileActive == CharonShip.Fragile)
                    {
                        g.tooltips.Add(
                            ttPos,
                            new GlossaryTooltip(
                                $"{ModEntry.Instance.Package.Manifest.UniqueName}::FragileDamageModifier"
                            )
                            {
                                Title = Localization.Str_ship_Charon_parttrait_Fragile_name(),
                                TitleColor = Colors.parttrait,
                                Description = Localization.Str_ship_Charon_parttrait_Fragile_description(),
                                Icon = Sprites.icons_fragile.Sprite,
                            }
                        );
                    }
                }
            )
            .Generate();
    }
}