using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

public static partial class Sprites
{
    public static ISpriteEntry artifacts_CrystalCore = null!;
    public static ISpriteEntry artifacts_CrystalCoreV2 = null!;
    public static ISpriteEntry artifacts_DeliveryNote = null!;
    public static ISpriteEntry artifacts_DoubleStar = null!;
    public static ISpriteEntry artifacts_DoubleStarInactive = null!;
    public static ISpriteEntry artifacts_FixedStar = null!;
    public static ISpriteEntry artifacts_FrostCannon = null!;
    public static ISpriteEntry artifacts_HeatShield = null!;
    public static ISpriteEntry artifacts_HeatShieldInactive = null!;
    public static ISpriteEntry artifacts_Piscium = null!;
    public static ISpriteEntry artifacts_RoutedCannon = null!;
    public static ISpriteEntry artifacts_RoutedCannonInactive = null!;
    public static ISpriteEntry artifacts_SaveState = null!;
    public static ISpriteEntry artifacts_ShrinkMechanism = null!;
    public static ISpriteEntry artifacts_ShrinkMechanismV2 = null!;
    public static ISpriteEntry artifacts_SiriusInquisitor = null!;
    public static ISpriteEntry artifacts_SiriusMissileBay = null!;
    public static ISpriteEntry artifacts_SiriusMissileBayV2 = null!;
    public static ISpriteEntry artifacts_SiriusSubwoofer = null!;
    public static ISpriteEntry artifacts_TinyWormhole = null!;
    public static ISpriteEntry artifacts_VolcanicShield = null!;
    public static ISpriteEntry cards_DodgeOrShift_Bottom = null!;
    public static ISpriteEntry cards_DodgeOrShift_Top = null!;
    public static ISpriteEntry cards_ShieldOrShot_Bottom = null!;
    public static ISpriteEntry cards_ShieldOrShot_Top = null!;
    public static ISpriteEntry cardShared_border_alpherg = null!;
    public static ISpriteEntry cardShared_border_sirius = null!;
    public static ISpriteEntry cardShared_border_spica = null!;
    public static ISpriteEntry drones_siriusDrone = null!;
    public static ISpriteEntry drones_siriusDroneMKII = null!;
    public static ISpriteEntry drones_siriusSemiDualDrone = null!;
    public static ISpriteEntry drones_siriusSemiDualDroneMKII = null!;
    public static ISpriteEntry icons_blue_zone = null!;
    public static ISpriteEntry icons_cold = null!;
    public static ISpriteEntry icons_freeze = null!;
    public static ISpriteEntry icons_frozen = null!;
    public static ISpriteEntry icons_orange_zone = null!;
    public static ISpriteEntry icons_power_down = null!;
    public static ISpriteEntry icons_siriusDrone = null!;
    public static ISpriteEntry icons_siriusDroneMkII = null!;
    public static ISpriteEntry icons_siriusSemiDualDrone = null!;
    public static ISpriteEntry icons_siriusSemiDualDroneMkII = null!;
    public static ISpriteEntry icons_sirius_toggle = null!;
    public static ISpriteEntry parts_albireo_cannon_left = null!;
    public static ISpriteEntry parts_albireo_cannon_left_inactive = null!;
    public static ISpriteEntry parts_albireo_cannon_right = null!;
    public static ISpriteEntry parts_albireo_cannon_right_inactive = null!;
    public static ISpriteEntry parts_albireo_chassis = null!;
    public static ISpriteEntry parts_albireo_cockpit = null!;
    public static ISpriteEntry parts_albireo_missilebay_inactive = null!;
    public static ISpriteEntry parts_albireo_missilebay_left = null!;
    public static ISpriteEntry parts_albireo_missilebay_right = null!;
    public static ISpriteEntry parts_alpherg_cannon = null!;
    public static ISpriteEntry parts_alpherg_cannon_inactive = null!;
    public static ISpriteEntry parts_alpherg_chassis = null!;
    public static ISpriteEntry parts_alpherg_chassis_left = null!;
    public static ISpriteEntry parts_alpherg_cockpit = null!;
    public static ISpriteEntry parts_alpherg_missilebay = null!;
    public static ISpriteEntry parts_alpherg_scaffold_blue = null!;
    public static ISpriteEntry parts_alpherg_scaffold_orange = null!;
    public static ISpriteEntry parts_alpherg_wing_left = null!;
    public static ISpriteEntry parts_alpherg_wing_right = null!;
    public static ISpriteEntry parts_gliese_cannon = null!;
    public static ISpriteEntry parts_gliese_cannon_temp = null!;
    public static ISpriteEntry parts_gliese_chassis = null!;
    public static ISpriteEntry parts_gliese_cockpit = null!;
    public static ISpriteEntry parts_gliese_missilebay = null!;
    public static ISpriteEntry parts_gliese_scaffolding_0 = null!;
    public static ISpriteEntry parts_gliese_scaffolding_1 = null!;
    public static ISpriteEntry parts_gliese_wings_0 = null!;
    public static ISpriteEntry parts_gliese_wings_1 = null!;
    public static ISpriteEntry parts_gliese_wings_2 = null!;
    public static ISpriteEntry parts_gliese_wings_3 = null!;
    public static ISpriteEntry parts_sirius_chassis = null!;
    public static ISpriteEntry parts_sirius_cockpit = null!;
    public static ISpriteEntry parts_sirius_comms = null!;
    public static ISpriteEntry parts_sirius_missilebay = null!;
    public static ISpriteEntry parts_sirius_missilebay_inactive = null!;
    public static ISpriteEntry parts_spica_cannon = null!;
    public static ISpriteEntry parts_spica_cockpit = null!;
    public static ISpriteEntry parts_spica_missilebay = null!;
    public static ISpriteEntry parts_spica_scaffolding = null!;
    public static ISpriteEntry parts_spica_tri_scaffolding = null!;
    public static ISpriteEntry parts_spica_wing_left = null!;
    public static ISpriteEntry parts_spica_wing_right = null!;
    public static ISpriteEntry parts_wolf_rayet_cannon = null!;
    public static ISpriteEntry parts_wolf_rayet_chassis = null!;
    public static ISpriteEntry parts_wolf_rayet_cockpit = null!;
    public static ISpriteEntry parts_wolf_rayet_missiles = null!;
    public static ISpriteEntry parts_wolf_rayet_missiles_inactive = null!;
    public static ISpriteEntry parts_wolf_rayet_misslebay = null!;
    public static ISpriteEntry parts_wolf_rayet_scaffolding = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        artifacts_CrystalCore = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/CrystalCore.png")
        );
        artifacts_CrystalCoreV2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/CrystalCoreV2.png")
        );
        artifacts_DeliveryNote = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DeliveryNote.png")
        );
        artifacts_DoubleStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStar.png")
        );
        artifacts_DoubleStarInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStarInactive.png")
        );
        artifacts_FixedStar = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FixedStar.png")
        );
        artifacts_FrostCannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FrostCannon.png")
        );
        artifacts_HeatShield = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/HeatShield.png")
        );
        artifacts_HeatShieldInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/HeatShieldInactive.png")
        );
        artifacts_Piscium = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/Piscium.png")
        );
        artifacts_RoutedCannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannon.png")
        );
        artifacts_RoutedCannonInactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannonInactive.png")
        );
        artifacts_SaveState = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SaveState.png")
        );
        artifacts_ShrinkMechanism = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/ShrinkMechanism.png")
        );
        artifacts_ShrinkMechanismV2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/ShrinkMechanismV2.png")
        );
        artifacts_SiriusInquisitor = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SiriusInquisitor.png")
        );
        artifacts_SiriusMissileBay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SiriusMissileBay.png")
        );
        artifacts_SiriusMissileBayV2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SiriusMissileBayV2.png")
        );
        artifacts_SiriusSubwoofer = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/SiriusSubwoofer.png")
        );
        artifacts_TinyWormhole = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/TinyWormhole.png")
        );
        artifacts_VolcanicShield = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/VolcanicShield.png")
        );
        cards_DodgeOrShift_Bottom = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Bottom.png")
        );
        cards_DodgeOrShift_Top = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/DodgeOrShift_Top.png")
        );
        cards_ShieldOrShot_Bottom = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/ShieldOrShot_Bottom.png")
        );
        cards_ShieldOrShot_Top = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cards/ShieldOrShot_Top.png")
        );
        cardShared_border_alpherg = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cardShared/border_alpherg.png")
        );
        cardShared_border_sirius = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cardShared/border_sirius.png")
        );
        cardShared_border_spica = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/cardShared/border_spica.png")
        );
        drones_siriusDrone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/drones/siriusDrone.png")
        );
        drones_siriusDroneMKII = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/drones/siriusDroneMKII.png")
        );
        drones_siriusSemiDualDrone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/drones/siriusSemiDualDrone.png")
        );
        drones_siriusSemiDualDroneMKII = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/drones/siriusSemiDualDroneMKII.png")
        );
        icons_blue_zone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/blue_zone.png")
        );
        icons_cold = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/cold.png")
        );
        icons_freeze = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/freeze.png")
        );
        icons_frozen = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/frozen.png")
        );
        icons_orange_zone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/orange_zone.png")
        );
        icons_power_down = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/power_down.png")
        );
        icons_siriusDrone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/siriusDrone.png")
        );
        icons_siriusDroneMkII = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/siriusDroneMkII.png")
        );
        icons_siriusSemiDualDrone = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/siriusSemiDualDrone.png")
        );
        icons_siriusSemiDualDroneMkII = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/siriusSemiDualDroneMkII.png")
        );
        icons_sirius_toggle = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/sirius_toggle.png")
        );
        parts_albireo_cannon_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left.png")
        );
        parts_albireo_cannon_left_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left_inactive.png")
        );
        parts_albireo_cannon_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right.png")
        );
        parts_albireo_cannon_right_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right_inactive.png")
        );
        parts_albireo_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_chassis.png")
        );
        parts_albireo_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cockpit.png")
        );
        parts_albireo_missilebay_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_inactive.png")
        );
        parts_albireo_missilebay_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_left.png")
        );
        parts_albireo_missilebay_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_right.png")
        );
        parts_alpherg_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon.png")
        );
        parts_alpherg_cannon_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cannon_inactive.png")
        );
        parts_alpherg_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis.png")
        );
        parts_alpherg_chassis_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_chassis_left.png")
        );
        parts_alpherg_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_cockpit.png")
        );
        parts_alpherg_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_missilebay.png")
        );
        parts_alpherg_scaffold_blue = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_blue.png")
        );
        parts_alpherg_scaffold_orange = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_scaffold_orange.png")
        );
        parts_alpherg_wing_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_left.png")
        );
        parts_alpherg_wing_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/alpherg_wing_right.png")
        );
        parts_gliese_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cannon.png")
        );
        parts_gliese_cannon_temp = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cannon_temp.png")
        );
        parts_gliese_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_chassis.png")
        );
        parts_gliese_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_cockpit.png")
        );
        parts_gliese_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_missilebay.png")
        );
        parts_gliese_scaffolding_0 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_scaffolding_0.png")
        );
        parts_gliese_scaffolding_1 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_scaffolding_1.png")
        );
        parts_gliese_wings_0 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_0.png")
        );
        parts_gliese_wings_1 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_1.png")
        );
        parts_gliese_wings_2 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_2.png")
        );
        parts_gliese_wings_3 = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/gliese_wings_3.png")
        );
        parts_sirius_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/sirius_chassis.png")
        );
        parts_sirius_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/sirius_cockpit.png")
        );
        parts_sirius_comms = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/sirius_comms.png")
        );
        parts_sirius_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/sirius_missilebay.png")
        );
        parts_sirius_missilebay_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/sirius_missilebay_inactive.png")
        );
        parts_spica_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_cannon.png")
        );
        parts_spica_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_cockpit.png")
        );
        parts_spica_missilebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_missilebay.png")
        );
        parts_spica_scaffolding = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_scaffolding.png")
        );
        parts_spica_tri_scaffolding = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_tri_scaffolding.png")
        );
        parts_spica_wing_left = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_left.png")
        );
        parts_spica_wing_right = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_right.png")
        );
        parts_wolf_rayet_cannon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_cannon.png")
        );
        parts_wolf_rayet_chassis = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_chassis.png")
        );
        parts_wolf_rayet_cockpit = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_cockpit.png")
        );
        parts_wolf_rayet_missiles = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_missiles.png")
        );
        parts_wolf_rayet_missiles_inactive = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_missiles_inactive.png")
        );
        parts_wolf_rayet_misslebay = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_misslebay.png")
        );
        parts_wolf_rayet_scaffolding = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/wolf_rayet_scaffolding.png")
        );
    }

}