using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class WolfRayetShip : IRegisterable
{
    internal static PStunMod HotStunModifier { get; private set; }
    internal static PType MissilePartType { get; private set; }
    internal static IPartEntry MissileSlot { get; private set; } = null!;
    internal static IPartEntry MissileEmptySlot { get; private set; } = null!;
	internal static IShipEntry WolfRayetEntry { get; private set; } = null!;
    private static string MissilePartTypeID = $"{ModEntry.Instance.Package.Manifest.UniqueName}::MissilePartType";

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        HotStunModifier = helper.Utilities.ObtainEnumCase<PStunMod>();
        MissilePartType = helper.Utilities.ObtainEnumCase<PType>();

        MissileSlot = helper.Content.Ships.RegisterPart("WolfRayetMissilesSlot", new()
        {
            Sprite = Sprites.wolf_rayet_missiles.Sprite,
            DisabledSprite = Sprites.wolf_rayet_missiles_inactive.Sprite
        });

        MissileEmptySlot = helper.Content.Ships.RegisterPart("WolfRayetMissilesEmptySlot", new()
        {
            Sprite = Sprites.wolf_rayet_scaffolding.Sprite,
            DisabledSprite = Sprites.wolf_rayet_scaffolding.Sprite
        });


        WolfRayetEntry = helper.Content.Ships.RegisterShip("WolfRayet", new()
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "description"]).Localize,
            UnderChassisSprite = Sprites.wolf_rayet_chassis.Sprite,
            Ship = new()
            {
                ship = new()
                {
                    hull = 15,
                    hullMax = 15,
                    shieldMaxBase = 8,
                    parts = [
                        new Part()
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("WolfRayetMissileBay", new ()
                            {
                                Sprite = Sprites.wolf_rayet_misslebay.Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("WolfRayetCockpit", new ()
                            {
                                Sprite = Sprites.wolf_rayet_cockpit.Sprite
                            }).UniqueName
                        },
                        new Part()
                        {
                            type = MissilePartType,
                            stunModifier = HotStunModifier,
                            skin = MissileSlot.UniqueName,
                            active = false
                        },
                        new Part()
                        {
                            type = MissilePartType,
                            stunModifier = HotStunModifier,
                            skin = MissileSlot.UniqueName,
                            active = false
                        },
                        new Part()
                        {
                            type = MissilePartType,
                            stunModifier = HotStunModifier,
                            skin = MissileSlot.UniqueName,
                            active = false
                        },
                        new Part()
                        {
                            type = PType.cannon,
                            skin = helper.Content.Ships.RegisterPart("WolfRayetCannon", new ()
                            {
                                Sprite = Sprites.wolf_rayet_cannon.Sprite
                            }).UniqueName,
                        },
                    ]
                },
                artifacts = [new ShieldPrep(), new HeatShield(), new DeliveryNote()],
                cards = [
                    new CannonColorless(),
                    new DodgeColorless(),
                    new BasicShieldColorless(),
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.RenderPartUI)),
            postfix: new HarmonyMethod(Ship_RenderPartUI_Postfix),
            transpiler: new HarmonyMethod(Ship_RenderPartUI_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.OnAfterTurn)),
            postfix: new HarmonyMethod(Ship_OnAfterTurn_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(AAttack_Begin_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(TTGlossary), nameof(TTGlossary.BuildIconAndText)),
            prefix: new HarmonyMethod(TTGlossary_BuildIconAndText_Prefix)
        );

        EnumExtensions.partStrs[MissilePartType] = MissilePartTypeID;
    }

    private static void Ship_OnAfterTurn_Postfix(Ship __instance, Combat c)
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

    private static bool TTGlossary_BuildIconAndText_Prefix(TTGlossary __instance, ref ValueTuple<Spr?, string> __result)
    {
        if (__instance.key == "part." + MissilePartTypeID)
        {
            __result = (null, ModEntry.Instance.Localizations.Localize(["parttype", "Missile", "name"]));
            return false;
        }

        return true;
    }

    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(
            MoveType.After,
            instr => instr.Match(OpCodes.Ldloc_3),
            instr => instr.MatchContains("hitShip"),
            instr => instr.Match(OpCodes.Brfalse)
        );

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_2);
        cursor.Emit(OpCodes.Ldarg_3);
        cursor.Emit(OpCodes.Ldloc_3);
        cursor.EmitDelegate((AAttack __instance, State s, Combat c, RaycastResult ray) =>
        {
            Ship target = (__instance.targetPlayer ? s.ship : c.otherShip);
            Part? part = target.GetPartAtWorldX(ray.worldX);
            if (part == null)
            {
                return;
            }

            if (!part.active)
            {
                part.active = true;
            }

            if (part.stunModifier == HotStunModifier)
            {
                target.Add(Status.heat, 1);
            }
        });

        return cursor.Generate();
    }

    private static void Ship_RenderPartUI_Postfix(Ship __instance, G g, Part part, int localX, string keyPrefix, bool isPreview)
    {
        if (part.invincible || part.stunModifier != HotStunModifier)
        {
            return;
        }

        if (g.boxes.FirstOrDefault(b => b.key == new UIKey(StableUK.part, localX, keyPrefix)) is not { } box)
        {
            return;
        }

        var offset = isPreview ? 25 : 34;
        var v = box.rect.xy + new Vec(0, __instance.isPlayerShip ? (offset - 16) : 8);

        var color = new Color(1, 1, 1, 0.8 + Math.Sin(g.state.time * 4.0) * 0.3);
        Draw.Sprite(StableSpr.icons_heat, v.x + 9, v.y, color: color);
    }

    private static IEnumerable<CodeInstruction> Ship_RenderPartUI_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var cursor = new ILCursor(generator, instructions);

        object? bb = null;

        cursor.GotoNext(
            instr => instr.Match(OpCodes.Ldc_I4_0),
            instr => instr.Match(OpCodes.Callvirt),
            instr => instr.MatchExtract(OpCodes.Stloc_S, out bb)
        );

        cursor.GotoNext(
            MoveType.After,
            instr => instr.Match(OpCodes.Ldarg_1),
            instr => instr.MatchContains("tutorial"),
            instr => instr.Match(OpCodes.Ldarg_1),
            instr => instr.Match(OpCodes.Ldarg_0),
            instr => instr.Match(OpCodes.Ldarg_3),
            instr => instr.Match(OpCodes.Ldloc_S),
            instr => instr.Match(OpCodes.Callvirt)
        );

        cursor.Emit(OpCodes.Ldarg_1);
        cursor.Emit(OpCodes.Ldarg_3);
        cursor.Emit(OpCodes.Ldloc_S, bb);
        cursor.EmitDelegate((G g, Part part, Box bb) =>
        {
            if (!bb.IsHover())
            {
                return;
            }

            Vec ttPos = bb.rect.xy + new Vec(16.0, 0.0);

            if (part.type == MissilePartType)
            {
                g.tooltips.Add(ttPos, new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::MissilePartType")
                {
                    Description = ModEntry.Instance.Localizations.Localize(["parttype", "Missile", "description"])
                });
            }

            if (part.stunModifier == HotStunModifier)
            {
                g.tooltips.Add(ttPos, new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::HotStunModifier")
                {
                    Title = ModEntry.Instance.Localizations.Localize(["parttrait", "Hot", "name"]),
                    TitleColor = Colors.parttrait,
                    Description = ModEntry.Instance.Localizations.Localize(["parttrait", "Hot", "description"]),
                    Icon = StableSpr.icons_heat
                });

                g.tooltips.Add(ttPos, new TTGlossary("status.heat", $"<c=boldPink>{3}</c>"));
            }
        });

        return cursor.Generate();
    }
}