using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class AlphergShip : IRegisterable
{
    internal static IPartEntry AlphergScaffoldOrange { get; set; } = null!;
    internal static IPartEntry AlphergScaffoldBlue { get; set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        AlphergScaffoldOrange = helper.Content.Ships.RegisterPart("AlphergScaffoldOrange", new () 
        {
            Sprite = Sprites.alpherg_scaffold_orange.Sprite
        });

        AlphergScaffoldBlue = helper.Content.Ships.RegisterPart("AlphergScaffoldBlue", new () 
        {
            Sprite = Sprites.alpherg_scaffold_blue.Sprite
        });

        var chassisSprite = helper.Content.Sprites.RegisterDynamicSprite("AlphergDynamicChassis", () => 
        {
            var state = MG.inst.g.state;
            if (!ModEntry.Instance.Helper.ModData.TryGetModData(state, "alpherg_chassis.activation", out bool leftActive))
            {
                return SpriteLoader.Get(Sprites.alpherg_chassis.Sprite)!;
            }
            if (leftActive)
            {
                return SpriteLoader.Get(Sprites.alpherg_chassis_left.Sprite)!;
            }

            return SpriteLoader.Get(Sprites.alpherg_chassis.Sprite)!;
        });


        helper.Content.Ships.RegisterShip("Alpherg", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "description"]).Localize,
            UnderChassisSprite = chassisSprite.Sprite,
            Ship = new() 
            {
                ship = new()
                {
                    hull = 12,
                    hullMax = 12,
                    shieldMaxBase = 6,
                    parts = [
                        new Part()
                        {
                            type = PType.wing,
                            damageModifier = PDamMod.armor,
                            skin = helper.Content.Ships.RegisterPart("AlphergWingLeft", new () 
                            {
                                Sprite = Sprites.alpherg_wing_left.Sprite
                            }).UniqueName
                        },
                        new Part()
                        {
                            type = PType.empty,
                            skin = AlphergScaffoldOrange.UniqueName,
                        },
                        new Part()
                        {
                            type = PType.cannon,
                            skin = helper.Content.Ships.RegisterPart("AlphergCannon", new () 
                            {
                                Sprite = Sprites.alpherg_cannon.Sprite,
                                DisabledSprite = Sprites.alpherg_cannon_inactive.Sprite
                            }).UniqueName,
                            active = false
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("AlphergCockpit", new () 
                            {
                                Sprite = Sprites.alpherg_cockpit.Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("AlphergBay", new () 
                            {
                                Sprite = Sprites.alpherg_missilebay.Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.wing,
                            skin = helper.Content.Ships.RegisterPart("AlphergWingRight", new () 
                            {
                                Sprite = Sprites.alpherg_wing_right.Sprite
                            }).UniqueName,
                        },
                    ]
                },
                artifacts = [new ShieldPrep(), new Piscium(), new RoutedCannon()],
                cards = [
                    new CannonColorless(),
                    new BasicShieldColorless(),
                    new DodgeColorless(),
                    new RerouteCannon()
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(AAttack_Begin_Transpiler)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            prefix: new HarmonyMethod(AAttack_Begin_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AVolleyAttackFromAllCannons), nameof(AVolleyAttackFromAllCannons.Begin)),
            postfix: new HarmonyMethod(AVolleyAttackFromAllCannons_Begin_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
            prefix: new HarmonyMethod(AAttack_GetTooltips_Prefix)
        );

        MethodInfo? info = null!;

        foreach (var nestedType in typeof(AAttack).GetNestedTypes())
        {
            foreach (var method in nestedType.GetMethods())
            {
                if (method.Name.Contains("<GetFromX>"))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var p = parameters[0];
                        if (p.ParameterType == typeof(Part))
                        {
                            info = method;
                        }
                    }

                }
            }
        }

        ModEntry.Instance.Harmony.Patch(
            info,
            prefix: new HarmonyMethod(AAttack_GetFromX_b__23_0_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts)),
            postfix: new HarmonyMethod(ArtifactReward_GetBlockedArtifacts_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(StoryNode), nameof(StoryNode.Filter)),
            prefix: new HarmonyMethod(StoryNode_Filter_Prefix)
        );
    }

    internal static bool StoryNode_Filter_Prefix(string key, State s, ref bool __result)
    {
        if (s.ship.key == $"{ModEntry.Instance.Package.Manifest.UniqueName}::Alpherg" && key == "AddScaffold")
        {
            __result = false;
            return false;
        }

        return true;
    }

    internal static void ArtifactReward_GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s) 
    {
        if (s.ship.key == $"{ModEntry.Instance.Package.Manifest.UniqueName}::Alpherg")
        {
            __result.Add(typeof(GlassCannon));
        }
    }

    private static State? state;
    private static AAttack? aAttack;

    internal static bool AAttack_GetFromX_b__23_0_Prefix(Part p, ref bool __result)
    {
        if (state is null || aAttack is null || aAttack.targetPlayer)
        {
            return true;
        }
        
        var routedCannon = state.EnumerateAllArtifacts().Where(x => x is RoutedCannon).Cast<RoutedCannon>().FirstOrDefault();

        if (routedCannon is not null && !routedCannon.disabled)
        {
            __result = (p.type == PType.empty || p.type == PType.cannon) && p.active;
            return false;
        }

        return true;
    }

    internal static void AAttack_Begin_Prefix(AAttack __instance, State s) 
    {
        state = s;
        aAttack = __instance;
    }

    internal static void AAttack_GetTooltips_Prefix(State s)
    {
        var routedCannon = s.EnumerateAllArtifacts().Where(x => x is RoutedCannon).Cast<RoutedCannon>().FirstOrDefault();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        int partX = s.ship.x;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                if (s.route is Combat combat && combat.stuff.TryGetValue(partX, out StuffBase? value))
                {
                    value.hilight = 2;
                }
                p.hilight = true;
            }
            partX++;
        }
    }


    internal static void AVolleyAttackFromAllCannons_Begin_Postfix(AVolleyAttackFromAllCannons __instance, State s, Combat c) 
    {
        var routedCannon = s.EnumerateAllArtifacts().Where(x => x is RoutedCannon).Cast<RoutedCannon>().FirstOrDefault();
        if (routedCannon is null || routedCannon.disabled)
        {
            return;
        }
        List<AAttack> listOfAttacks = new List<AAttack>();
        int i = 0;
        foreach (Part p in s.ship.parts)
        {
            if (p.type == PType.empty && p.active)
            {
                __instance.attack.fromX = new int?(i);
                listOfAttacks.Add(Mutil.DeepCopy(__instance.attack));
            }
            i++;
        }

        c.QueueImmediate(listOfAttacks);
    }

    internal static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) 
    {
        var cursor = new ILCursor(generator, instructions);

        cursor.GotoNext(
            MoveType.After,
            instr => instr.MatchContains("GetPartTypeCount")
        );

        cursor.Emit(OpCodes.Ldarg_0);
        cursor.Emit(OpCodes.Ldarg_2);
        cursor.EmitDelegate((int x, AAttack aAttack, State s) => {
            if (aAttack.targetPlayer)
            {
                return x;
            }
            var routedCannon = s.EnumerateAllArtifacts().Where(x => x is RoutedCannon).Cast<RoutedCannon>().FirstOrDefault();
            if (routedCannon is null || routedCannon.disabled)
            {
                return x;
            }

            int cannon = s.ship.GetPartTypeCount(PType.cannon, false);
            int empty = s.ship.GetPartTypeCount(PType.empty, false);
            
            return cannon + empty;
        });

        return cursor.Generate();
    }
}