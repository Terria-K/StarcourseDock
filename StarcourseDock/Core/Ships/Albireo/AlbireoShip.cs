using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class AlbireoShip : IRegisterable
{
    internal static IPartEntry AlbireoMissileBayBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoCannonBlue { get; private set; } = null!;
    internal static IPartEntry AlbireoCockpitBlue { get; private set; } = null!;
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Albireo", "name"])
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(["ship", "Albireo", "description"])
                    .Localize,
                UnderChassisSprite = Sprites.parts_albireo_chassis.Sprite,
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
                                key = "missiles"
                            },
                            new Part()
                            {
                                type = PType.cannon,
                                skin = AlbireoCannonBlue.UniqueName,
                                key = "cannon"
                            },
                            new Part()
                            {
                                type = PType.cockpit,
                                skin = AlbireoCockpitBlue.UniqueName,
                                key = "cockpit"
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
