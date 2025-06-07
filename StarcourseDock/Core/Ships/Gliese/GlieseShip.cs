using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class GlieseShip : IRegisterable
{
    internal static IShipEntry GlieseEntry { get; private set; } = null!;
    internal static IPartEntry GlieseCannonTemp { get; private set; } = null!;
    internal static IPartEntry GliesePortal { get; private set; } = null!;
    internal static IPartEntry GlieseCockpit { get; private set; } = null!;
    internal static IPartEntry GlieseScaffolding1 { get; private set; } = null!;
    internal static IPartEntry GlieseScaffolding2 { get; private set; } = null!;
    internal static IPartEntry GlieseCrystal1 { get; private set; } = null!;
    internal static IPartEntry GlieseCrystal2 { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        GlieseScaffolding1 = helper.Content.Ships.RegisterPart(
            "GlieseScaffolding_crystal0",
            new() { Sprite = Sprites.parts_gliese_scaffolding_0.Sprite }
        );

        GlieseScaffolding2 = helper.Content.Ships.RegisterPart(
            "GlieseScaffolding_crystal1",
            new() { Sprite = Sprites.parts_gliese_scaffolding_1.Sprite }
        );

        GliesePortal = helper.Content.Ships.RegisterPart(
            "GliesePortal",
            new() { Sprite = Sprites.parts_gliese_wings_3.Sprite }
        );

        GlieseCannonTemp = helper.Content.Ships.RegisterPart(
            "GliesecrystalCannon",
            new() { Sprite = Sprites.parts_gliese_cannon_temp.Sprite }
        );

        GlieseCockpit = helper.Content.Ships.RegisterPart(
            "GlieseCockpit",
            new() { Sprite = Sprites.parts_gliese_cockpit.Sprite }
        );

        GlieseCrystal1 = helper.Content.Ships.RegisterPart(
            "Gliesecrystal1",
            new() { Sprite = Sprites.parts_gliese_wings_0.Sprite }
        );

        GlieseCrystal2 = helper.Content.Ships.RegisterPart(
            "Gliesecrystal2",
            new() { Sprite = Sprites.parts_gliese_wings_1.Sprite }
        );

        GlieseEntry = helper.Content.Ships.RegisterShip(
            "Gliese",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "name"]).Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "description"])
                    .Localize,
                UnderChassisSprite = Sprites.parts_empty_chassis.Sprite,
                Ship = new()
                {
                    ship = new()
                    {
                        x = 2,
                        hull = 11,
                        hullMax = 11,
                        shieldMaxBase = 5,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.special,
                                skin = GlieseCrystal1.UniqueName,
                                stunModifier = PStunMod.breakable,
                                key = "crystal1::StarcourseDock",
                            },
                            new Part()
                            {
                                type = PType.missiles,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "GlieseMissileBay",
                                        new() { Sprite = Sprites.parts_gliese_missilebay.Sprite }
                                    )
                                    .UniqueName,
                            },
                            new Part()
                            {
                                type = PType.cannon,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "GlieseCannon",
                                        new() { Sprite = Sprites.parts_gliese_cannon.Sprite }
                                    )
                                    .UniqueName,
                            },
                            new Part() { type = PType.cockpit, skin = GlieseCockpit.UniqueName },
                            new Part() { type = PType.empty, skin = GlieseScaffolding2.UniqueName },
                            new Part()
                            {
                                type = PType.wing,
                                key = "portal",
                                damageModifier = PDamMod.armor,
                                skin = GliesePortal.UniqueName,
                            },
                        ],
                    },
                    artifacts = [new ShieldPrep(), new CrystalCore(), new FrostCannon()],
                    cards =
                    [
                        new BasicShieldColorless(),
                        new DodgeColorless(),
                        new CannonColorless(),
                        new Unfreeze(),
                    ],
                },
            }
        );
    }
}
