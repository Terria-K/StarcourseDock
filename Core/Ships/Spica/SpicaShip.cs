using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal sealed class SpicaShip : IRegisterable
{
    internal static IPartEntry SpicaScaffold { get; private set; } = null!;
    internal static IPartEntry SpicaCannon { get; private set; } = null!;
	internal static IPartEntry SpicaCockpit { get; private set; } = null!;
	internal static IShipEntry SpicaEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SpicaScaffold = helper.Content.Ships.RegisterPart("SpicaScaffolding", new () 
        {
            Sprite = Sprites.spica_scaffolding.Sprite
        });

        SpicaCannon = helper.Content.Ships.RegisterPart("SpicaCannon", new () 
        {
            Sprite = Sprites.spica_cannon.Sprite
        });

        SpicaCockpit = helper.Content.Ships.RegisterPart("SpicaCockpit", new () 
        {
            Sprite = Sprites.spica_cockpit.Sprite
        });

            
        SpicaEntry = helper.Content.Ships.RegisterShip("Spica", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "description"]).Localize,
            UnderChassisSprite = Sprites.gliese_chassis.Sprite,
            Ship = new() 
            {
                ship = new()
                {
                    x = 3,
                    hull = 13,
                    hullMax = 13,
                    shieldMaxBase = 7,
                    parts = [
                        new Part()
                        {
                            type = PType.wing,
                            damageModifier = PDamMod.armor,
                            skin = helper.Content.Ships.RegisterPart("SpicaWingLeft", new () 
                            {
                                Sprite = Sprites.spica_wing_left.Sprite
                            }).UniqueName
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = SpicaCockpit.UniqueName
                        },
                        new Part()
                        {
                            type = PType.cannon,
                            skin = SpicaCannon.UniqueName,
                            key = "closeToScaffold"
                        },
                        new Part()
                        {
                            type = PType.empty,
                            skin = SpicaScaffold.UniqueName,
                            key = "toRemove"
                        },
                        new Part()
                        {
                            type = PType.missiles,
                            skin = helper.Content.Ships.RegisterPart("SpicaMissileBay", new () 
                            {
                                Sprite = Sprites.spica_missilebay.Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.wing,
                            damageModifier = PDamMod.armor,
                            skin = helper.Content.Ships.RegisterPart("SpicaWingRight", new () 
                            {
                                Sprite = Sprites.spica_wing_right.Sprite
                            }).UniqueName,
                        },
                    ]
                },
                artifacts = [new ShieldPrep(), new FixedStar(), new SaveState()],
                cards = [
                    new ShieldOrShot(),
                    new DodgeOrShift()
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts)),
            postfix: new HarmonyMethod(ArtifactReward_GetBlockedArtifacts_Postfix)
        );
    }

    internal static void ArtifactReward_GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s) 
    {
        if (s.ship.key == SpicaEntry.UniqueName)
        {
            __result.Add(typeof(AdaptivePlating));
        }
    }
}