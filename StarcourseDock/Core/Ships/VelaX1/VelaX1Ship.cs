using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class VelaX1Ship : IRegisterable
{
    public static IShipEntry VelaX1Entry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        //VelaX1Entry = helper.Content.Ships.RegisterShip(
        //    "VelaX1",
        //    new() 
        //    {
        //        Name = Localization.ship_VelaX1_name(),
        //        Description = Localization.ship_VelaX1_description(),
        //        Ship = new()
        //    }
        //);
    }
}
