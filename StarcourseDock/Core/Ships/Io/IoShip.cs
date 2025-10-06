using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class IoShip : IRegisterable
{
    public static IShipEntry IoShipEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        IoShipEntry = helper.Content.Ships.RegisterShip(
            "Io",
            new() 
            {
                Name = Localization.ship_Io_name(),
                Description = Localization.ship_Io_description(),
                Ship = new()
            }
        );
    }
}
