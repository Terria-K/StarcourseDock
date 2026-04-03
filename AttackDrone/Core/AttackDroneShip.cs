using System.Collections.Frozen;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.AttackDrone;

internal static class AttackDroneShip
{
    public static IPartEntry AttackDroneSkin { get; private set; } = null!;
    public static IPartEntry AttackDroneIISkin { get; private set; } = null!;
    public static IShipEntry ShipEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var emptyChassisSprite = helper.Content.Sprites.RegisterSprite(
            "EmptyChassis",
            package.PackageRoot.GetRelativeFile("assets/empty_chassis.png")
        );

        var droneSprite = helper.Content.Sprites.RegisterSprite(
            "Drone",
            package.PackageRoot.GetRelativeFile("assets/drone.png")
        );

        var drone2Sprite = helper.Content.Sprites.RegisterSprite(
            "DroneII",
            package.PackageRoot.GetRelativeFile("assets/droneII.png")
        );

        AttackDroneSkin = helper.Content.Ships.RegisterPart("AttackDroneCannon", new()
        {
            Sprite = droneSprite.Sprite
        });

        AttackDroneIISkin = helper.Content.Ships.RegisterPart("AttackDroneCannonII", new()
        {
            Sprite = drone2Sprite.Sprite
        });

        ShipEntry = helper.Content.Ships.RegisterShip(
            "AttackDrone",
            new()
            {
                UnderChassisSprite = emptyChassisSprite.Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "AttackDrone", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "AttackDrone", "description"]).Localize,
                ExclusiveArtifactTypes = new HashSet<Type>() { typeof(DroneMkI), typeof(DroneMkII) }.ToFrozenSet(),
                Ship = new()
                {
                    cards = [new CannonColorless(), new CannonColorless(), new DodgeColorless(), new DodgeColorless()],
                    artifacts = [new ShieldPrep(), new DroneMkI()],
                    ship = new()
                    {
                        hullMax = 1,
                        hull = 1,
                        evadeMax = 1,
                        shieldMaxBase = 0,

                        parts =
                        [
                            new Part()
                            {
                                type = PType.cannon,
                                skin = AttackDroneSkin.UniqueName
                            }
                        ]
                    }
                }
            }
        );
    }
}
