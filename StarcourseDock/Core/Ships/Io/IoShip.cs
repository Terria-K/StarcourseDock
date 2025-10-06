using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class IoShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(DroneMania),
        typeof(GravitationalPull),
        typeof(AsteroidAirlock),
    ];

    public static IShipEntry IoShipEntry { get; private set; } = null!;
    public static IPartEntry IoBayLeft { get; private set; } = null!;
    public static IPartEntry IoBayRight { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        IoBayLeft = helper.Content.Ships.RegisterPart(
            "IoBayLeft",
            new() { Sprite = Sprites.parts_io_bay_left.Sprite }
        );
        
        IoBayRight = helper.Content.Ships.RegisterPart(
            "IoBayRight",
            new() { Sprite = Sprites.parts_io_bay_right.Sprite }
        );

        var IoEmpty = helper.Content.Ships.RegisterPart(
            "IoEmpty",
            new() { Sprite = Sprites.parts_io_empty.Sprite }
        );

        IoShipEntry = helper.Content.Ships.RegisterShip(
            "Io",
            new() 
            {
                Name = Localization.ship_Io_name(),
                Description = Localization.ship_Io_description(),
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),
                UnderChassisSprite = Sprites.parts_io_chassis.Sprite,
                Ship = new()
                {
                    ship = new()
                    {
                        x = 3,
                        hull = 11,
                        hullMax = 11,
                        shieldMaxBase = 4,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.wing,
                                skin = helper.Content.Ships
                                    .RegisterPart(
                                        "IoWingLeft", 
                                        new () { Sprite = Sprites.parts_io_wing_left.Sprite })
                                    .UniqueName,
                            },

                            new Part()
                            {
                                type = PType.missiles,
                                skin = IoBayLeft.UniqueName,
                                key = "bayleft"
                            },

                            new Part()
                            {
                                type = PType.empty,
                                skin = IoEmpty
                                    .UniqueName
                            },

                            new Part()
                            {
                                type = PType.cockpit,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "IoCockpit",
                                        new() { Sprite = Sprites.parts_io_cockpit.Sprite }
                                    )
                                    .UniqueName,
                                key = "iocenter"
                            },

                            new Part()
                            {
                                type = PType.empty,
                                skin = IoEmpty
                                    .UniqueName
                            },

                            new Part()
                            {
                                type = PType.missiles,
                                skin = IoBayRight.UniqueName,
                                key = "bayright"
                            },

                            new Part()
                            {
                                type = PType.wing,
                                skin = helper.Content.Ships
                                    .RegisterPart(
                                        "IoWingRight", 
                                        new () { Sprite = Sprites.parts_io_wing_right.Sprite })
                                    .UniqueName,
                            }
                        ],
                    },
                    artifacts = [new ShieldPrep(), new DroneMania(), new GravitationalPull()],
                    cards =
                    [
                        new CannonColorless(),
                        new DroneshiftColorless(),
                        new DodgeColorless(),
                        new BasicShieldColorless()
                    ],
                },
            }
        );
    }
}
