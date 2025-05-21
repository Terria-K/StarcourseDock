using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class GlieseShip : IRegisterable
{
	internal static IShipEntry GlieseEntry { get; private set; } = null!;
    internal static IPartEntry GlieseCrystal1 { get; private set; } = null!;
    internal static IPartEntry GlieseCrystal2 { get; private set; } = null!;
    internal static IPartEntry GlieseCrystal3 { get; private set; } = null!;
    internal static IPartEntry GlieseScaffolding1 { get; private set; } = null!;
    internal static IPartEntry GlieseScaffolding2 { get; private set; } = null!;
    internal static IPartEntry GlieseCannonTemp { get; private set; } = null!;
    internal static IPartEntry GliesePortal { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        GlieseCrystal1 = helper.Content.Ships.RegisterPart("Gliesecrystal1", new() 
        {
            Sprite = Sprites.gliese_wings_0.Sprite
        });

        GlieseCrystal2 = helper.Content.Ships.RegisterPart("Gliesecrystal2", new() 
        {
            Sprite = Sprites.gliese_wings_1.Sprite
        });

        GlieseCrystal3 = helper.Content.Ships.RegisterPart("Gliesecrystal3", new() 
        {
            Sprite = Sprites.gliese_wings_2.Sprite
        });

        GlieseScaffolding1 = helper.Content.Ships.RegisterPart("GlieseScaffolding_crystal0", new() 
        {
            Sprite = Sprites.gliese_scaffolding_0.Sprite
        });

        GlieseScaffolding2 = helper.Content.Ships.RegisterPart("GlieseScaffolding_crystal1", new() 
        {
            Sprite = Sprites.gliese_scaffolding_1.Sprite
        });

        GliesePortal = helper.Content.Ships.RegisterPart("GliesePortal", new() 
        {
            Sprite = Sprites.gliese_wings_3.Sprite
        });

        GlieseCannonTemp = helper.Content.Ships.RegisterPart("GliesecrystalCannon", new() 
        {
            Sprite = Sprites.gliese_cannon_temp.Sprite
        });

        GlieseEntry = helper.Content.Ships.RegisterShip("Gliese", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "description"]).Localize,
            UnderChassisSprite = Sprites.gliese_chassis.Sprite,
            Ship = new() 
            {
                ship = new()
                {
                    x = 2,
                    hull = 10,
                    hullMax = 10,
                    shieldMaxBase = 5,
                    parts = [
                        new Part() 
                        {
                            type = PType.special,
                            skin = GlieseCrystal1.UniqueName,
                            stunModifier = PStunMod.breakable,
                            key = "crystal1::StarcourseDock"
                        },
                        new Part() 
                        {
                            type = PType.special,
                            skin = GlieseCrystal2.UniqueName,
                            stunModifier = PStunMod.breakable,
                            key = "crystal2::StarcourseDock"
                        },
                        new Part() 
                        {
                            type = PType.cannon,
                            skin = helper.Content.Ships.RegisterPart("GlieseCannon_crystal", new() 
                            {
                                Sprite = Sprites.gliese_cannon.Sprite
                            }).UniqueName
                        },
                        new Part() 
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("GlieseCockpit_crystal", new() 
                            {
                                Sprite = Sprites.gliese_cockpit.Sprite
                            }).UniqueName
                        },
                        new Part() 
                        {
                            type = PType.special,
                            skin = GlieseCrystal3.UniqueName,
                            stunModifier = PStunMod.breakable,
                            key = "crystal3::StarcourseDock"
                        },
                        new Part() 
                        {
                            type = PType.empty,
                            skin = GlieseScaffolding1.UniqueName
                        },
                        new Part() 
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("GlieseMissileBay_crystal", new() 
                            {
                                Sprite = Sprites.gliese_missilebay.Sprite
                            }).UniqueName
                        },
                        new Part() 
                        {
                            type = PType.empty,
                            skin = GlieseScaffolding2.UniqueName
                        },
                        new Part() 
                        {
                            type = PType.special,
                            skin = GlieseCrystal2.UniqueName,
                            stunModifier = PStunMod.breakable,
                            key = "crystal4::StarcourseDock"
                        },
                        new Part() 
                        {
                            type = PType.wing,
                            key = "portal",
                            damageModifier = PDamMod.armor,
                            skin = GliesePortal.UniqueName
                        },
                    ]
                },
                artifacts = [new ShieldPrep(), new CrystalCore(), new FrostCannon()],
                cards = [
                    new BasicShieldColorless(),
                    new DodgeColorless(),
                    new CannonColorless()
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts)),
            postfix: new HarmonyMethod(ArtifactReward_GetBlockedArtifacts)
        );

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(Part), nameof(Part.GetBrokenPartSkin)),
            postfix: new HarmonyMethod(Part_GetBrokenPartSkin_Postfix)
        );
    }

    private static void ArtifactReward_GetBlockedArtifacts(HashSet<Type> __result, State s)
    {
        if (s.ship.key != GlieseEntry.UniqueName)
        {
            __result.Add(typeof(CrystalCoreV2));
        }
        if (s.ship.key == GlieseEntry.UniqueName)
        {
            __result.Add(typeof(StunCalibrator));
        }
    }

    private static void Part_GetBrokenPartSkin_Postfix(Part __instance, ref string __result)
    {
        switch (__instance.key)
        {
        case "crystal1::StarcourseDock":
            __result = GlieseScaffolding1.UniqueName;
            break;
        case "crystal2::StarcourseDock":
            __result = GlieseScaffolding2.UniqueName;
            break;
        case "crystal3::StarcourseDock":
            __result = GlieseScaffolding2.UniqueName;
            break;
        case "crystal4::StarcourseDock":
            __result = GlieseScaffolding1.UniqueName;
            break;
        }
    }
}