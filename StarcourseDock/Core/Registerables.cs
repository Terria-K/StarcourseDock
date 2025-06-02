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
        Teuria.StarcourseDock.CrystalCore.Register(package, helper);
        Teuria.StarcourseDock.CrystalCoreV2.Register(package, helper);
        Teuria.StarcourseDock.FrostCannon.Register(package, helper);
        Teuria.StarcourseDock.AbsoluteZero.Register(package, helper);
        Teuria.StarcourseDock.Unfreeze.Register(package, helper);
        Teuria.StarcourseDock.GlieseShip.Register(package, helper);
        Teuria.StarcourseDock.SiriusKit.Register(package, helper);
        Teuria.StarcourseDock.SiriusInquisitor.Register(package, helper);
        Teuria.StarcourseDock.SiriusMissileBay.Register(package, helper);
        Teuria.StarcourseDock.SiriusMissileBayV2.Register(package, helper);
        Teuria.StarcourseDock.SiriusSubwoofer.Register(package, helper);
        Teuria.StarcourseDock.BarrageMode.Register(package, helper);
        Teuria.StarcourseDock.SiriusBusiness.Register(package, helper);
        Teuria.StarcourseDock.SiriusQuestion.Register(package, helper);
        Teuria.StarcourseDock.ToggleMissileBay.Register(package, helper);
        Teuria.StarcourseDock.SiriusShip.Register(package, helper);
        Teuria.StarcourseDock.BayPowerDownStatus.Register(package, helper);
        Teuria.StarcourseDock.FixedStar.Register(package, helper);
        Teuria.StarcourseDock.ShrinkMechanism.Register(package, helper);
        Teuria.StarcourseDock.ShrinkMechanismV2.Register(package, helper);
        Teuria.StarcourseDock.TinyWormhole.Register(package, helper);
        Teuria.StarcourseDock.DodgeOrShift.Register(package, helper);
        Teuria.StarcourseDock.ShieldOrShot.Register(package, helper);
        Teuria.StarcourseDock.Shrink.Register(package, helper);
        Teuria.StarcourseDock.SpicaShip.Register(package, helper);
        Teuria.StarcourseDock.DeliveryNote.Register(package, helper);
        Teuria.StarcourseDock.HeatShield.Register(package, helper);
        Teuria.StarcourseDock.HeatShieldV2.Register(package, helper);
        Teuria.StarcourseDock.WolfRayetShip.Register(package, helper);
        Teuria.StarcourseDock.ColdStatus.Register(package, helper);
    }
}