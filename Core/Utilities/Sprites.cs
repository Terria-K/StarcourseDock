using Nickel;
using Nanoray.PluginManager;

namespace Teuria.StarcourseDock;

public static partial class Sprites 
{
    public static ISpriteEntry DoubleStar = null!;
    public static ISpriteEntry DoubleStarInactive = null!;
    public static ISpriteEntry FixedStar = null!;
    public static ISpriteEntry Piscium = null!;
    public static ISpriteEntry RoutedCannon = null!;
    public static ISpriteEntry RoutedCannonInactive = null!;
    public static ISpriteEntry DodgeOrShift_Bottom = null!;
    public static ISpriteEntry DodgeOrShift_Top = null!;
    public static ISpriteEntry ShieldOrShot_Bottom = null!;
    public static ISpriteEntry ShieldOrShot_Top = null!;
    public static ISpriteEntry border_alpherg = null!;
    public static ISpriteEntry border_spica = null!;
    public static ISpriteEntry blue_zone = null!;
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
    public static ISpriteEntry spica_cannon = null!;
    public static ISpriteEntry spica_chassis = null!;
    public static ISpriteEntry spica_cockpit = null!;
    public static ISpriteEntry spica_missilebay = null!;
    public static ISpriteEntry spica_scaffolding = null!;
    public static ISpriteEntry spica_wing_left = null!;
    public static ISpriteEntry spica_wing_right = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        DoubleStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStar.png")
        );
        DoubleStarInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStarInactive.png")
        );
        FixedStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FixedStar.png")
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
    }

}