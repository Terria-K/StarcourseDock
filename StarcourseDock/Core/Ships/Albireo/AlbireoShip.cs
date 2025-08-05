using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;
using System.Collections.Frozen;

namespace Teuria.StarcourseDock;


internal sealed class AlbireoShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(DoubleDeck),
        typeof(PolarityWings)
    ];

    internal static IPartEntry AlbireoMissileBayBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoCannonBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoCockpitBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoMissileBayOrange { get; private set; } = null!;
    internal static IPartEntry AlbireoCannonOrange { get; private set; } = null!;
    internal static IPartEntry AlbireoCockpitOrange { get; private set; } = null!;
    internal static IPartEntry AlbireoWingsBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoWingsOrange { get; private set; } = null!;
    internal static IPartEntry AlbireoEmptyBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoEmptyOrange { get; private set; } = null!;
    internal static IShipEntry AlbireoShipEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        AlbireoMissileBayBlue = helper.Content.Ships.RegisterPart(
            nameof(AlbireoMissileBayBlue),
            new()
            {
                Sprite = Sprites.parts_albireo_missilebay.Sprite
            }
        );

        AlbireoCannonBlue = helper.Content.Ships.RegisterPart(
            nameof(AlbireoCannonBlue),
            new()
            {
                Sprite = Sprites.parts_albireo_cannon.Sprite
            }
        );

        AlbireoCockpitBlue = helper.Content.Ships.RegisterPart(
            nameof(AlbireoCockpitBlue),
            new()
            {
                Sprite = Sprites.parts_albireo_cockpit.Sprite
            }
        );

        AlbireoMissileBayOrange = helper.Content.Ships.RegisterPart(
            nameof(AlbireoMissileBayOrange),
            new()
            {
                Sprite = Sprites.parts_albireo_missilebay_orange.Sprite
            }
        );

        AlbireoCannonOrange = helper.Content.Ships.RegisterPart(
            nameof(AlbireoCannonOrange),
            new()
            {
                Sprite = Sprites.parts_albireo_cannon_orange.Sprite
            }
        );

        AlbireoCockpitOrange = helper.Content.Ships.RegisterPart(
            nameof(AlbireoCockpitOrange),
            new()
            {
                Sprite = Sprites.parts_albireo_cockpit_orange.Sprite
            }
        );

        AlbireoWingsBlue = helper.Content.Ships.RegisterPart(
            nameof(AlbireoWingsBlue),
            new()
            {
                Sprite = Sprites.parts_albireo_wings_blue.Sprite
            }
        );

        AlbireoWingsOrange = helper.Content.Ships.RegisterPart(
            nameof(AlbireoWingsOrange),
            new()
            {
                Sprite = Sprites.parts_albireo_wings_orange.Sprite
            }
        );

        AlbireoEmptyBlue = helper.Content.Ships.RegisterPart(
            nameof(AlbireoEmptyBlue),
            new()
            {
                Sprite = Sprites.parts_albireo_empty_blue.Sprite
            }
        );

        AlbireoEmptyOrange = helper.Content.Ships.RegisterPart(
            nameof(AlbireoEmptyOrange),
            new()
            {
                Sprite = Sprites.parts_albireo_empty_orange.Sprite
            }
        );

        AlbireoShipEntry = helper.Content.Ships.RegisterShip(
            "Albireo",
            new()
            {
                Name = Localization.ship_Albireo_name(),
                Description = Localization.ship_Albireo_description(),
                UnderChassisSprite = Sprites.parts_albireo_chassis.Sprite,
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),
                Ship = new()
                {
                    ship = new()
                    {
                        x = 7,
                        hull = 12,
                        hullMax = 12,
                        shieldMaxBase = 4,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.wing,
                                skin = AlbireoWingsBlue.UniqueName,
                                key = "blue_wing"
                            },
                            new Part()
                            {
                                type = PType.missiles,
                                skin = AlbireoMissileBayBlue.UniqueName,
                                key = "ab_missiles"
                            },
                            new Part()
                            {
                                type = PType.cannon,
                                skin = AlbireoCannonBlue.UniqueName,
                                key = "ab_cannon"
                            },
                            new Part()
                            {
                                type = PType.cockpit,
                                skin = AlbireoCockpitBlue.UniqueName,
                                key = "ab_cockpit"
                            },
                            new Part()
                            {
                                type = PType.wing,
                                skin = AlbireoWingsOrange.UniqueName,
                                key = "orange_wing"
                            },
                        ],
                    },

                    artifacts = [new ShieldPrep(), new DoubleDeck()],
                    cards =
                    [
                        new DodgeColorless(),
                        new CannonColorless(),
                        new BasicShieldColorless()
                    ],
                },
            }
        );
    }
}
