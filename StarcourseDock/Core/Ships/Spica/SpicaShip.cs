using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SpicaShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(ShrinkMechanism),
        typeof(ShrinkMechanismV2),
        typeof(TinyWormhole),
        typeof(FixedStar),
    ];

    internal static IPartEntry SpicaScaffold { get; private set; } = null!;
    internal static IPartEntry SpicaTriScaffold { get; private set; } = null!;
    internal static IPartEntry SpicaCannon { get; private set; } = null!;
    internal static IPartEntry SpicaCockpit { get; private set; } = null!;
    internal static IShipEntry SpicaEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SpicaScaffold = helper.Content.Ships.RegisterPart(
            "SpicaScaffolding",
            new() { Sprite = Sprites.parts_spica_scaffolding.Sprite }
        );

        SpicaTriScaffold = helper.Content.Ships.RegisterPart(
            "SpicaTriScaffolding",
            new() { Sprite = Sprites.parts_spica_tri_scaffolding.Sprite }
        );

        var spicaSeleneScaffold = helper.Content.Ships.RegisterPart(
            "SpicaSeleneScaffolding",
            new() { Sprite = Sprites.parts_spica_selene_scaffolding.Sprite }
        );

        SpicaCannon = helper.Content.Ships.RegisterPart(
            "SpicaCannon",
            new() { Sprite = Sprites.parts_spica_cannon.Sprite }
        );

        SpicaCockpit = helper.Content.Ships.RegisterPart(
            "SpicaCockpit",
            new() { Sprite = Sprites.parts_spica_cockpit.Sprite }
        );

        SpicaEntry = helper.Content.Ships.RegisterShip(
            "Spica",
            new()
            {
                Name = Localization.ship_Spica_name(),
                Description = Localization.ship_Spica_description(),
                UnderChassisSprite = Sprites.parts_empty_chassis.Sprite,
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),
                Ship = new()
                {
                    ship = new()
                    {
                        x = 3,
                        hull = 13,
                        hullMax = 13,
                        shieldMaxBase = 6,
                        parts =
                        [
                            new Part()
                            {
                                type = PType.wing,
                                damageModifier = PDamMod.armor,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "SpicaWingLeft",
                                        new() { Sprite = Sprites.parts_spica_wing_left.Sprite }
                                    )
                                    .UniqueName,
                                key = "Starcourse::leftwing",
                            },
                            new Part()
                            {
                                type = PType.cockpit,
                                skin = SpicaCockpit.UniqueName,
                                key = "cockpit",
                            },
                            new Part()
                            {
                                type = PType.cannon,
                                skin = SpicaCannon.UniqueName,
                                key = "closeToScaffold",
                            },
                            new Part()
                            {
                                type = PType.empty,
                                skin = SpicaScaffold.UniqueName,
                                key = "toRemove",
                            },
                            new Part()
                            {
                                type = PType.missiles,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "SpicaMissileBay",
                                        new() { Sprite = Sprites.parts_spica_missilebay.Sprite }
                                    )
                                    .UniqueName,
                                key = "missiles",
                            },
                            new Part()
                            {
                                type = PType.wing,
                                damageModifier = PDamMod.armor,
                                skin = helper
                                    .Content.Ships.RegisterPart(
                                        "SpicaWingRight",
                                        new() { Sprite = Sprites.parts_spica_wing_right.Sprite }
                                    )
                                    .UniqueName,
                                key = "Starcourse::rightwing",
                            },
                        ],
                    },
                    artifacts = [new ShieldPrep(), new ShrinkMechanism(), new FixedStar()],
                    cards = [new ShieldOrShot(), new DodgeOrShift()],
                },
            }
        );

        SpicaEntry.AddSeleneScaffold(new() { Part = spicaSeleneScaffold });
    }
}
