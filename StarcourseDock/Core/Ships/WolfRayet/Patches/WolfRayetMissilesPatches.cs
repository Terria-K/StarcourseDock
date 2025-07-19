using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;
using Nickel;

namespace Teuria.StarcourseDock;

internal partial class WolfRayetMissilesPatches : IPatchable
{
    [OnPostfix<Ship>(nameof(Ship.OnAfterTurn))]
    private static void OnAfterTurn_Postfix(Ship __instance, Combat c)
    {
        if (__instance.GetPartTypeCount(WolfRayetShip.MissilePartType) == 0)
        {
            return;
        }

        if (__instance.Get(Status.heat) >= __instance.heatTrigger)
        {
            c.Queue(new ALaunchMissiles() { targetPlayer = __instance.isPlayerShip });
        }
    }

    [OnPrefix<Ship>(nameof(Ship.NormalDamage))]
    private static void Ship_NormalDamage_Prefix(Ship __instance, int? maybeWorldGridX)
    {
        if (maybeWorldGridX is null)
        {
            return;
        }

        Part? part = __instance.GetPartAtWorldX(maybeWorldGridX.Value);

        if (part == null)
        {
            return;
        }

        if (part.type == WolfRayetShip.MissilePartType && !part.active)
        {
            part.active = true;
        }

        if (part.stunModifier == WolfRayetShip.HotStunModifier)
        {
            __instance.Add(Status.heat, 1);
        }
    }

    [OnPostfix<Ship>(nameof(Ship.RenderPartUI))]
    private static void Ship_RenderPartUI_Postfix(
        Ship __instance,
        G g,
        Part part,
        int localX,
        string keyPrefix,
        bool isPreview
    )
    {
        if (part.invincible || part.stunModifier != WolfRayetShip.HotStunModifier)
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
        Draw.Sprite(StableSpr.icons_heat, v.x + 9, v.y, color: color);
    }

    [OnTranspiler<Ship>(nameof(Ship.RenderPartUI))]
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

                    if (part.type == WolfRayetShip.MissilePartType)
                    {
                        g.tooltips.Add(
                            ttPos,
                            new GlossaryTooltip(
                                $"{ModEntry.Instance.Package.Manifest.UniqueName}::MissilePartType"
                            )
                            {
                                Description = Localization.Str_ship_WolfRayet_parttype_Missile_description(),
                            }
                        );
                    }

                    if (part.stunModifier == WolfRayetShip.HotStunModifier)
                    {
                        g.tooltips.Add(
                            ttPos,
                            new GlossaryTooltip(
                                $"{ModEntry.Instance.Package.Manifest.UniqueName}::HotStunModifier"
                            )
                            {
                                Title = Localization.Str_ship_WolfRayet_parttrait_Hot_name(),
                                TitleColor = Colors.parttrait,
                                Description = Localization.Str_ship_WolfRayet_parttrait_Hot_description(),
                                Icon = StableSpr.icons_heat,
                            }
                        );

                        g.tooltips.Add(
                            ttPos,
                            new TTGlossary("status.heat", $"<c=boldPink>{g.state.ship.heatMin}</c>")
                        );
                    }
                }
            )
            .Generate();
    }

    [OnPrefix<TTGlossary>(nameof(TTGlossary.BuildIconAndText))]
    private static bool TTGlossary_BuildIconAndText_Prefix(
        TTGlossary __instance,
        ref ValueTuple<Spr?, string> __result
    )
    {
        if (__instance.key == "part." + WolfRayetShip.MissilePartTypeID)
        {
            __result = (
                null,
                Localization.Str_ship_WolfRayet_parttype_Missile_name()
            );
            return false;
        }

        return true;
    }
}
