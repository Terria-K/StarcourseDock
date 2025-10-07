using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class WolfRayetShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(HeatShield),
        typeof(HeatShieldV2),
        typeof(DeliveryNote),
        typeof(LaunchCodes),
        typeof(SeriousDedication),
        typeof(LifesEnd)
    ];

    internal static PStunMod HotStunModifier { get; private set; }
    internal static PType MissilePartType { get; private set; }
    internal static IPartEntry MissileSlot { get; private set; } = null!;
    internal static IPartEntry MissileLeftSlot { get; private set; } = null!;
    internal static IPartEntry MissileRightSlot { get; private set; } = null!;
    internal static IPartEntry MissileEmptySlot { get; private set; } = null!;
    internal static IPartEntry MissileLeftEmptySlot { get; private set; } = null!;
    internal static IPartEntry MissileRightEmptySlot { get; private set; } = null!;
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

        MissileLeftSlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesLeftSlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_bay_missile.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_bay_missile_inactive.Sprite,
            }
        );

        MissileRightSlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesRightSlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_bay_missile_right.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_bay_missile_right_inactive.Sprite,
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

        MissileLeftEmptySlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesLeftEmptySlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_bay_empty.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_bay_empty.Sprite,
            }
        );

        MissileRightEmptySlot = helper.Content.Ships.RegisterPart(
            "WolfRayetMissilesRightEmptySlot",
            new()
            {
                Sprite = Sprites.parts_wolf_rayet_bay_empty_right.Sprite,
                DisabledSprite = Sprites.parts_wolf_rayet_bay_empty_right.Sprite,
            }
        );

        WolfRayetEntry = helper.Content.Ships.RegisterShip(
            "WolfRayet",
            new()
            {
                Name = Localization.ship_WolfRayet_name(),
                Description = Localization.ship_WolfRayet_description(),
                UnderChassisSprite = Sprites.parts_wolf_rayet_chassis.Sprite,
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),
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
                                key = "rayet_missiles",
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
                                key = "rayet_cannon",
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
