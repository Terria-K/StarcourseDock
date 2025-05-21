using Nickel;
using Nanoray.PluginManager;

namespace Teuria.StarcourseDock;

internal static class Registerables
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Teuria.StarcourseDock.SaveState.Register(package, helper);
        Teuria.StarcourseDock.AlphergShip.Register(package, helper);
        Teuria.StarcourseDock.Piscium.Register(package, helper);
        Teuria.StarcourseDock.RoutedCannon.Register(package, helper);
        Teuria.StarcourseDock.RerouteCannon.Register(package, helper);
        Teuria.StarcourseDock.CrystalCore.Register(package, helper);
        Teuria.StarcourseDock.CrystalCoreV2.Register(package, helper);
        Teuria.StarcourseDock.FrostCannon.Register(package, helper);
        Teuria.StarcourseDock.AbsoluteZero.Register(package, helper);
        Teuria.StarcourseDock.GlieseShip.Register(package, helper);
        Teuria.StarcourseDock.FixedStar.Register(package, helper);
        Teuria.StarcourseDock.ShrinkMechanism.Register(package, helper);
        Teuria.StarcourseDock.ShrinkMechanismV2.Register(package, helper);
        Teuria.StarcourseDock.DodgeOrShift.Register(package, helper);
        Teuria.StarcourseDock.ShieldOrShot.Register(package, helper);
        Teuria.StarcourseDock.Shrink.Register(package, helper);
        Teuria.StarcourseDock.SpicaShip.Register(package, helper);
        Teuria.StarcourseDock.DeliveryNote.Register(package, helper);
        Teuria.StarcourseDock.HeatShield.Register(package, helper);
        Teuria.StarcourseDock.WolfRayetShip.Register(package, helper);
        Teuria.StarcourseDock.ColdStatus.Register(package, helper);
    }
}