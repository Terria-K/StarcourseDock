using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;


internal sealed class AtlasShip : IRegisterable
{
    internal static IReadOnlyList<Type> ExclusiveArtifacts => [
        typeof(GyroscopeEngine),
    ];

    internal static IShipEntry AtlasEntry { get; set; } = null!;

    internal static PStunMod LeftMoveStunModifier { get; private set; }

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        LeftMoveStunModifier = helper.Utilities.ObtainEnumCase<PStunMod>();

        var empty = helper.Content.Ships.RegisterPart("AtlasEmpty", new() 
        {
            Sprite = Sprites.parts_atlas_empty.Sprite
        });

        AtlasEntry = helper.Content.Ships.RegisterShip("Atlas", new()
        {
            Name = Localization.ship_Atlas_name(),
            Description = Localization.ship_Atlas_description(),
            Ship = new()
            {
                ship = new()
                {
                    hull = 7,
                    hullMax = 7,
                    shieldMaxBase = 2,
                    
                    parts = [
                        new()
                        {
                            type = PType.wing,
                            skin = helper.Content.Ships.RegisterPart("AtlasWing", new() 
                            {
                                Sprite = Sprites.parts_atlas_wing.Sprite
                            }).UniqueName,
                            stunModifier = LeftMoveStunModifier
                        },
                        new()
                        {
                            type = PType.empty,
                            skin = empty.UniqueName
                        },
                        new()
                        {
                            type = PType.empty,
                            skin = empty.UniqueName
                        },
                        new()
                        {
                            type = PType.cannon,
                            skin = helper.Content.Ships.RegisterPart("AtlasCannon", new() 
                            {
                                Sprite = Sprites.parts_atlas_cannon.Sprite
                            }).UniqueName
                        },
                        new() 
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("AtlasCockpit", new() 
                            {
                                Sprite = Sprites.parts_atlas_cockpit.Sprite
                            }).UniqueName
                        },
                        new() 
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("AtlasMissiles", new() 
                            {
                                Sprite = Sprites.parts_atlas_missiles.Sprite
                            }).UniqueName
                        }
                    ]
                },

                artifacts = [new ShieldPrep(), new GyroscopeEngine()],
                cards = [new CannonColorless(), new DodgeColorless(), new BasicShieldColorless()]
            },

            UnderChassisSprite = Sprites.parts_atlas_chassis.Sprite,
            ExclusiveArtifactTypes = ExclusiveArtifacts.ToFrozenSet()
        });

        AtlasEntry.AddSeleneScaffold(new() { Part = empty });
    }
}