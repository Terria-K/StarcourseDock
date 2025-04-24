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
    internal static ISpriteEntry AlphergChassisRight { get; set; } = null!;
    internal static ISpriteEntry AlphergChassisLeft { get; set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        AlphergScaffoldOrange =  helper.Content.Ships.RegisterPart("AlphergScaffoldOrange", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_orange.png")
            ).Sprite
        });

        AlphergScaffoldBlue =  helper.Content.Ships.RegisterPart("AlphergScaffoldBlue", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_blue.png")
            ).Sprite
        });

        AlphergChassisRight = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis.png"));
        AlphergChassisLeft = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis_left.png"));

        helper.Content.Ships.RegisterShip("Alpherg", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "description"]).Localize,
            UnderChassisSprite = AlphergChassisRight.Sprite,
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
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_left.png")
                                ).Sprite
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
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon.png")
                                ).Sprite,
                                DisabledSprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon_inactive.png")
                                ).Sprite
                            }).UniqueName,
                            active = false
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("AlphergCockpit", new () 
                            {
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cockpit.png")
                                ).Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("AlphergBay", new () 
                            {
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_missilebay.png")
                                ).Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.wing,
                            skin = helper.Content.Ships.RegisterPart("AlphergWingRight", new () 
                            {
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_right.png")
                                ).Sprite
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