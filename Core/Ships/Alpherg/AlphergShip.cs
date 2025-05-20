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
    internal static IShipEntry AlphergEntry { get; set; } = null!;

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


        AlphergEntry = helper.Content.Ships.RegisterShip("Alpherg", new () 
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
            original: AccessTools.DeclaredMethod(typeof(StoryNode), nameof(StoryNode.Filter)),
            prefix: new HarmonyMethod(StoryNode_Filter_Prefix)
        );

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(State), nameof(State.MakeRunWinRoute)),
            postfix: new HarmonyMethod(State_MakeRunWinRoute_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(State), nameof(State.EndRun)),
            postfix: new HarmonyMethod(State_EndRun)
        );
    }

    internal static void State_EndRun(State __instance)
    {
        if (__instance.ship != null && __instance.ship.key == AlphergEntry.UniqueName)
        {
            ModEntry.Instance.Helper.ModData.SetModData(__instance, "alpherg_chassis.activation", false);
        }
    }

    internal static void State_MakeRunWinRoute_Postfix(State __instance)
    {
        if (__instance.ship != null && __instance.ship.key == AlphergEntry.UniqueName)
        {
            ModEntry.Instance.Helper.ModData.SetModData(__instance, "alpherg_chassis.activation", false);
        }
    }

    internal static bool StoryNode_Filter_Prefix(string key, State s, ref bool __result)
    {
        if (s.ship.key == AlphergEntry.UniqueName && key == "AddScaffold")
        {
            __result = false;
            return false;
        }

        return true;
    }
}