using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SpicaShip : IRegisterable
{
    internal static IPartEntry SpicaScaffold { get; private set; } = null!;
    internal static IPartEntry SpicaCannon { get; private set; } = null!;
	internal static IPartEntry SpicaCockpit { get; private set; } = null!;
	internal static IShipEntry ShipEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        SpicaScaffold = helper.Content.Ships.RegisterPart("SpicaScaffolding", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/spica_scaffolding.png")
            ).Sprite
        });

        SpicaCannon = helper.Content.Ships.RegisterPart("SpicaCannon", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/spica_cannon.png")
            ).Sprite
        });

        SpicaCockpit = helper.Content.Ships.RegisterPart("SpicaCockpit", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/spica_cockpit.png")
            ).Sprite
        });

            
        ShipEntry = helper.Content.Ships.RegisterShip("Spica", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "description"]).Localize,
            UnderChassisSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/parts/spica_chassis.png")).Sprite,
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
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_left.png")
                                ).Sprite
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
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/spica_missilebay.png")
                                ).Sprite
                            }).UniqueName,
                        },
                        new Part()
                        {
                            type = PType.wing,
                            damageModifier = PDamMod.armor,
                            skin = helper.Content.Ships.RegisterPart("SpicaWingRight", new () 
                            {
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/spica_wing_right.png")
                                ).Sprite
                            }).UniqueName,
                        },
                    ]
                },
                artifacts = [new ShieldPrep(), new FixedStar()],
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

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.DrawTopLayer)),
            postfix: new HarmonyMethod(Ship_DrawTopLayer_Postfix)
        );
    }

    internal static void ArtifactReward_GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s) 
    {
        if (s.ship.key == $"{ModEntry.Instance.Package.Manifest.UniqueName}::Spica")
        {
            __result.Add(typeof(AdaptivePlating));
        }
    }

    internal static void Ship_DrawTopLayer_Postfix(Ship __instance, G g, Vec v, Vec worldPos) 
    {
        if (!__instance.isPlayerShip)
        {
            return;
        }
        for (int i = 0; i < __instance.parts.Count; i++)
        {
            var part = __instance.parts[i];
            Vec partPos = worldPos + new Vec((part.xLerped ?? i) * 16.0, -32.0 + (__instance.isPlayerShip ? part.offset.y : (1.0 + -part.offset.y)));
			Vec screenPos = v + partPos;
            if (part.skin == SpicaCockpit.UniqueName)
            {
                Vec glowPos = screenPos + new Vec(8.0, 32 + 12 * (__instance.isPlayerShip ? (-1) : 1));
                Glow.Draw(glowPos + new Vec(0, 5), 30.0, new Color("c5a7e5"));
            }
        }
    }
}
