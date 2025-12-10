using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CharonShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(WrathDrill),
        typeof(CrimsonCannon),
        typeof(SanityExpansion),
        typeof(EnrageDrill)
    ];

    public static PDamMod Fragile { get; private set; }
    public static IShipEntry CharonEntry { get; private set; } = null!;
    private static List<Spr>? drillSprites;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Fragile = helper.Utilities.ObtainEnumCase<PDamMod>();
        var charonCannon = helper.Content.Sprites.RegisterDynamicSprite(() =>
        {
            var global = MG.inst.g;

            var s = global.state;

            drillSprites ??= [
                    Sprites.parts_charon_cannon1.Sprite,
                    Sprites.parts_charon_cannon2.Sprite,
                    Sprites.parts_charon_cannon3.Sprite,
                    Sprites.parts_charon_cannon4.Sprite,
                    Sprites.parts_charon_cannon5.Sprite
                ];

            return SpriteLoader.Get(drillSprites.GetModulo((int)(s.time * 12.0)))!;
        });

        var charonEmpty = helper.Content.Ships.RegisterPart("CharonEmpty", new()
        {
            Sprite = Sprites.parts_charon_empty.Sprite
        });

        CharonEntry = helper.Content.Ships.RegisterShip(
            "Charon",
            new() 
            {
                Name = Localization.ship_Charon_name(),
                Description = Localization.ship_Charon_description(),
                UnderChassisSprite = Sprites.parts_charon_chassis.Sprite,
                ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet(),

                Ship = new()
                {
                    ship = new()
                    {
                        x = 1,
                        hull = 12,
                        hullMax = 12,
                        shieldMaxBase = 4,

                        parts = [
                            new Part()
                            {
                                type = PType.wing,
                                skin = helper.Content.Ships.RegisterPart("CharonWingLeft", new() 
                                {
                                    Sprite = Sprites.parts_charon_wing_left.Sprite
                                }).UniqueName
                            },

                            new Part()
                            {
                                type = PType.cannon,
                                skin = helper.Content.Ships.RegisterPart("CharonCannon", new()
                                {
                                    Sprite = charonCannon.Sprite
                                }).UniqueName
                            },

                            new Part() 
                            {
                                type = PType.cockpit,
                                skin = helper.Content.Ships.RegisterPart("CharonCockpit", new()
                                {
                                    Sprite = Sprites.parts_charon_cockpit.Sprite
                                }).UniqueName
                            },

                            new Part()
                            {
                                type = PType.empty,
                                skin = charonEmpty.UniqueName
                            },

                            new Part()
                            {
                                type = PType.missiles,
                                skin = helper.Content.Ships.RegisterPart("CharonMissiles", new()
                                {
                                    Sprite = Sprites.parts_charon_misslebay.Sprite
                                }).UniqueName
                            },

                            new Part()
                            {
                                type = PType.wing,
                                skin = helper.Content.Ships.RegisterPart("CharonWingRight", new()
                                {
                                    Sprite = Sprites.parts_charon_wing_right.Sprite
                                }).UniqueName
                            }
                        ]
                    },

                    artifacts = [new ShieldPrep(), new WrathDrill()],
                    cards = [
                        new CannonColorless(), 
                        new BasicShieldColorless(), 
                        new DodgeColorless()
                    ]
                }
            }
        );

        CharonEntry.RegisterAddScaffold(new() { Part = charonEmpty });
    }
}
