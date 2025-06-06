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

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        GliesePortal = helper.Content.Ships.RegisterPart(
            "GliesePortal",
            new() { Sprite = Sprites.parts_gliese_wings_3.Sprite }
        );

        GlieseCannonTemp = helper.Content.Ships.RegisterPart(
            "GliesecrystalCannon",
            new() { Sprite = Sprites.parts_gliese_cannon_temp.Sprite }
        );

        GlieseCockpit = helper.Content.Ships.RegisterPart(
            "GlieseMissileBay",
            new() { Sprite = Sprites.parts_gliese_cockpit.Sprite }
        );

        GlieseEntry = helper.Content.Ships.RegisterShip(
            "Gliese",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "name"]).Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Gliese", "description"])
                    .Localize,
                UnderChassisSprite = Sprites.parts_gliese_chassis.Sprite,
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
                                skin = "crystal_1",
                                stunModifier = PStunMod.breakable,
                                key = "crystal1::StarcourseDock",
                            },
                            new Part()
                            {
                                type = PType.missiles,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "GlieseCockpit",
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
                            new Part() { type = PType.empty, skin = "empty_crystal_2" },
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
