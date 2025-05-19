using Nickel;
using Nanoray.PluginManager;

namespace Teuria.StarcourseDock;

public static partial class Sprites 
{
    public static ISpriteEntry CrystalCore = null!;
    public static ISpriteEntry CrystalCoreV2 = null!;
    public static ISpriteEntry DeliveryNote = null!;
    public static ISpriteEntry DoubleStar = null!;
    public static ISpriteEntry DoubleStarInactive = null!;
    public static ISpriteEntry FixedStar = null!;
    public static ISpriteEntry FrostCannon = null!;
    public static ISpriteEntry HeatShield = null!;
    public static ISpriteEntry HeatShieldInactive = null!;
    public static ISpriteEntry Piscium = null!;
    public static ISpriteEntry RoutedCannon = null!;
    public static ISpriteEntry RoutedCannonInactive = null!;
    public static ISpriteEntry SaveState = null!;
    public static ISpriteEntry DodgeOrShift_Bottom = null!;
    public static ISpriteEntry DodgeOrShift_Top = null!;
    public static ISpriteEntry ShieldOrShot_Bottom = null!;
    public static ISpriteEntry ShieldOrShot_Top = null!;
    public static ISpriteEntry border_alpherg = null!;
    public static ISpriteEntry border_spica = null!;
    public static ISpriteEntry blue_zone = null!;
    public static ISpriteEntry cold = null!;
    public static ISpriteEntry freeze = null!;
    public static ISpriteEntry orange_zone = null!;
    public static ISpriteEntry albireo_cannon_left = null!;
    public static ISpriteEntry albireo_cannon_left_inactive = null!;
    public static ISpriteEntry albireo_cannon_right = null!;
    public static ISpriteEntry albireo_cannon_right_inactive = null!;
    public static ISpriteEntry albireo_chassis = null!;
    public static ISpriteEntry albireo_cockpit = null!;
    public static ISpriteEntry albireo_missilebay_inactive = null!;
    public static ISpriteEntry albireo_missilebay_left = null!;
    public static ISpriteEntry albireo_missilebay_right = null!;
    public static ISpriteEntry alpherg_cannon = null!;
    public static ISpriteEntry alpherg_cannon_inactive = null!;
    public static ISpriteEntry alpherg_chassis = null!;
    public static ISpriteEntry alpherg_chassis_left = null!;
    public static ISpriteEntry alpherg_cockpit = null!;
    public static ISpriteEntry alpherg_missilebay = null!;
    public static ISpriteEntry alpherg_scaffold_blue = null!;
    public static ISpriteEntry alpherg_scaffold_orange = null!;
    public static ISpriteEntry alpherg_wing_left = null!;
    public static ISpriteEntry alpherg_wing_right = null!;
    public static ISpriteEntry gliese_cannon = null!;
    public static ISpriteEntry gliese_cannon_temp = null!;
    public static ISpriteEntry gliese_chassis = null!;
    public static ISpriteEntry gliese_cockpit = null!;
    public static ISpriteEntry gliese_missilebay = null!;
    public static ISpriteEntry gliese_scaffolding_0 = null!;
    public static ISpriteEntry gliese_scaffolding_1 = null!;
    public static ISpriteEntry gliese_wings_0 = null!;
    public static ISpriteEntry gliese_wings_1 = null!;
    public static ISpriteEntry gliese_wings_2 = null!;
    public static ISpriteEntry gliese_wings_3 = null!;
    public static ISpriteEntry spica_cannon = null!;
    public static ISpriteEntry spica_chassis = null!;
    public static ISpriteEntry spica_cockpit = null!;
    public static ISpriteEntry spica_missilebay = null!;
    public static ISpriteEntry spica_scaffolding = null!;
    public static ISpriteEntry spica_wing_left = null!;
    public static ISpriteEntry spica_wing_right = null!;
    public static ISpriteEntry wolf_rayet_cannon = null!;
    public static ISpriteEntry wolf_rayet_chassis = null!;
    public static ISpriteEntry wolf_rayet_cockpit = null!;
    public static ISpriteEntry wolf_rayet_missiles = null!;
    public static ISpriteEntry wolf_rayet_missiles_inactive = null!;
    public static ISpriteEntry wolf_rayet_misslebay = null!;
    public static ISpriteEntry wolf_rayet_scaffolding = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        CrystalCore = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/CrystalCore.png")
        );
        CrystalCoreV2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/CrystalCoreV2.png")
        );
        DeliveryNote = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DeliveryNote.png")
        );
        DoubleStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStar.png")
        );
        DoubleStarInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStarInactive.png")
        );
        FixedStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FixedStar.png")
        );
        FrostCannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FrostCannon.png")
        );
        HeatShield = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/HeatShield.png")
        );
        HeatShieldInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/HeatShieldInactive.png")
        );
        Piscium = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/Piscium.png")
        );
        RoutedCannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannon.png")
        );
        RoutedCannonInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannonInactive.png")
        );
        SaveState = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SaveState.png")
        );
        DodgeOrShift_Bottom = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Bottom.png")
        );
        DodgeOrShift_Top = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Top.png")
        );
        ShieldOrShot_Bottom = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/ShieldOrShot_Bottom.png")
        );
        ShieldOrShot_Top = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/ShieldOrShot_Top.png")
        );
        border_alpherg = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cardShared/border_alpherg.png")
        );
        border_spica = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cardShared/border_spica.png")
        );
        blue_zone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/blue_zone.png")
        );
        cold = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/cold.png")
        );
        freeze = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/freeze.png")
        );
        orange_zone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/orange_zone.png")
        );
        albireo_cannon_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left.png")
        );
        albireo_cannon_left_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left_inactive.png")
        );
        albireo_cannon_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right.png")
        );
        albireo_cannon_right_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right_inactive.png")
        );
        albireo_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_chassis.png")
        );
        albireo_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cockpit.png")
        );
        albireo_missilebay_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_inactive.png")
        );
        albireo_missilebay_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_left.png")
        );
        albireo_missilebay_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_right.png")
        );
        alpherg_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon.png")
        );
        alpherg_cannon_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon_inactive.png")
        );
        alpherg_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis.png")
        );
        alpherg_chassis_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis_left.png")
        );
        alpherg_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cockpit.png")
        );
        alpherg_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_missilebay.png")
        );
        alpherg_scaffold_blue = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_blue.png")
        );
        alpherg_scaffold_orange = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_orange.png")
        );
        alpherg_wing_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_left.png")
        );
        alpherg_wing_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_right.png")
        );
        gliese_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cannon.png")
        );
        gliese_cannon_temp = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cannon_temp.png")
        );
        gliese_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_chassis.png")
        );
        gliese_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cockpit.png")
        );
        gliese_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_missilebay.png")
        );
        gliese_scaffolding_0 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_scaffolding_0.png")
        );
        gliese_scaffolding_1 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_scaffolding_1.png")
        );
        gliese_wings_0 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_0.png")
        );
        gliese_wings_1 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_1.png")
        );
        gliese_wings_2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_2.png")
        );
        gliese_wings_3 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_3.png")
        );
        spica_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_cannon.png")
        );
        spica_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_chassis.png")
        );
        spica_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_cockpit.png")
        );
        spica_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_missilebay.png")
        );
        spica_scaffolding = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_scaffolding.png")
        );
        spica_wing_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_left.png")
        );
        spica_wing_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_right.png")
        );
        wolf_rayet_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_cannon.png")
        );
        wolf_rayet_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_chassis.png")
        );
        wolf_rayet_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_cockpit.png")
        );
        wolf_rayet_missiles = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_missiles.png")
        );
        wolf_rayet_missiles_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_missiles_inactive.png")
        );
        wolf_rayet_misslebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_misslebay.png")
        );
        wolf_rayet_scaffolding = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_scaffolding.png")
        );
    }

}