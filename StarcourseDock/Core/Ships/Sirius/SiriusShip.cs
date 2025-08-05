using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(SiriusInquisitor),
        typeof(SiriusMissileBay),
        typeof(SiriusMissileBayV2),
        typeof(SiriusSubwoofer),
    ];

    internal static IShipEntry SiriusEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        var bay = helper.Content.Ships.RegisterPart(
            "SiriusMissileBay",
            new()
            {
                Sprite = Sprites.parts_sirius_missilebay.Sprite,
                DisabledSprite = Sprites.parts_sirius_missilebay_inactive.Sprite,
            }
        );

        SiriusEntry = helper.Content.Ships.RegisterShip(
            "Sirius",
            new()
            {
                Name = Localization.ship_Sirius_name(),
                Description = Localization.ship_Sirius_description(),
                UnderChassisSprite = Sprites.parts_sirius_chassis.Sprite,
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),
                Ship = new()
                {
                    ship = new()
                    {
                        x = 1,
                        hull = 9,
                        hullMax = 9,
                        shieldMaxBase = 5,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.missiles,
                                skin = bay.UniqueName,
                                damageModifier = PDamMod.armor,
                                damageModifierOverrideWhileActive = PDamMod.none,
                                key = "firstBay",
                            },
                            new Part()
                            {
                                type = PType.cockpit,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "SiriusCockpit",
                                        new() { Sprite = Sprites.parts_sirius_cockpit.Sprite }
                                    )
                                    .UniqueName,
                                key = "cockpit",
                            },
                            new Part()
                            {
                                type = PType.comms,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "SiriusComms",
                                        new() { Sprite = Sprites.parts_sirius_comms.Sprite }
                                    )
                                    .UniqueName,
                                key = "closeToScaffold",
                                damageModifier = PDamMod.weak,
                            },
                            new Part()
                            {
                                type = PType.missiles,
                                skin = bay.UniqueName,
                                key = "weak",
                                damageModifier = PDamMod.armor,
                                damageModifierOverrideWhileActive = PDamMod.none,
                                active = false,
                            },
                        ],
                    },
                    artifacts = [new ShieldPrep(), new SiriusMissileBay()],
                    cards =
                    [
                        new SiriusBusiness(),
                        new DodgeColorless(),
                        new BasicShieldColorless(),
                        new DroneshiftColorless(),
                        new CannonColorless(),
                    ],
                },
            }
        );
    }
}
