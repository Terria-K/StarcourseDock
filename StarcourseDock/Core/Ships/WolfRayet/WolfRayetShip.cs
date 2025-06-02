using System.Reflection.Emit;
using CutebaltCore;
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
    internal static string MissilePartTypeID =
        $"{ModEntry.Instance.Package.Manifest.UniqueName}::MissilePartType";

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        HotStunModifier = helper.Utilities.ObtainEnumCase<PStunMod>();
        MissilePartType = helper.Utilities.ObtainEnumCase<PType>();

        MissileSlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesSlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_missiles.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_missiles_inactive.Sprite,
            }
        );

        MissileEmptySlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesEmptySlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_scaffolding.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_scaffolding.Sprite,
            }
        );

        WolfRayetEntry = helper.Content.Ships.RegisterShip(
            "WolfRayet",
            new()
            {
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "name"])
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "description"])
                    .Localize,
                UnderChassisSprite = Sprites.parts_wolf_rayet_chassis.Sprite,
                Ship = new()
                {
                    ship = new()
                    {
                        hull = 15,
                        hullMax = 15,
                        shieldMaxBase = 8,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.missiles,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "WolfRayetMissileBay",
                                        new() { Sprite = Sprites.parts_wolf_rayet_misslebay.Sprite }
                                    )
                                    .UniqueName,
                            },
                            new Part()
                            {
                                type = PType.cockpit,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "WolfRayetCockpit",
                                        new() { Sprite = Sprites.parts_wolf_rayet_cockpit.Sprite }
                                    )
                                    .UniqueName,
                            },
                            new Part()
                            {
                                type = MissilePartType,
                                stunModifier = HotStunModifier,
                                skin = MissileSlot.UniqueName,
                                active = false,
                            },
                            new Part()
                            {
                                type = MissilePartType,
                                stunModifier = HotStunModifier,
                                skin = MissileSlot.UniqueName,
                                active = false,
                            },
                            new Part()
                            {
                                type = MissilePartType,
                                stunModifier = HotStunModifier,
                                skin = MissileSlot.UniqueName,
                                active = false,
                            },
                            new Part()
                            {
                                type = PType.cannon,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "WolfRayetCannon",
                                        new() { Sprite = Sprites.parts_wolf_rayet_cannon.Sprite }
                                    )
                                    .UniqueName,
                            },
                        ],
                    },
                    artifacts = [new ShieldPrep(), new HeatShield(), new DeliveryNote()],
                    cards =
                    [
                        new CannonColorless(),
                        new DodgeColorless(),
                        new BasicShieldColorless(),
                    ],
                },
            }
        );

        EnumExtensions.partStrs[MissilePartType] = MissilePartTypeID;
    }
}
