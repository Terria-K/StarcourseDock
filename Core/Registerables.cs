using Nickel;
using Nanoray.PluginManager;

namespace Teuria.StarcourseDock;


internal static class Registerables
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Teuria.StarcourseDock.AlphergShip.Register(package, helper);
        Teuria.StarcourseDock.Piscium.Register(package, helper);
        Teuria.StarcourseDock.RoutedCannon.Register(package, helper);
        Teuria.StarcourseDock.RerouteCannon.Register(package, helper);
        Teuria.StarcourseDock.FixedStar.Register(package, helper);
        Teuria.StarcourseDock.DodgeOrShift.Register(package, helper);
        Teuria.StarcourseDock.ShieldOrShot.Register(package, helper);
        Teuria.StarcourseDock.Shrink.Register(package, helper);
        Teuria.StarcourseDock.SpicaShip.Register(package, helper);
    }
}