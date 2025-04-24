using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class AlbireoShip : IRegisterable
{
    private static readonly UK AlbireoAUK = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();
    private static readonly UK AlbireoBUK = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();
    internal static IPartEntry AlbireoCannonLeft { get; private set; } = null!;
    internal static IPartEntry AlbireoCannonRight { get; private set; } = null!;
    internal static IPartEntry AlbireoMissileBayLeft { get; private set; } = null!;
    internal static IPartEntry AlbireoMissileBayRight { get; private set; } = null!;
	internal static IShipEntry ShipEntry { get; private set; } = null!;

    internal static ISpriteEntry AlbireoAIcon { get; private set; } = null!;
    internal static ISpriteEntry AlbireoBIcon { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        AlbireoAIcon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/blue_zone.png")
        );
        AlbireoBIcon = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/icons/orange_zone.png")
        );
        var inactiveMissileBaySprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_inactive.png")
        ).Sprite;

        var inactiveCannonLeft = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left_inactive.png")
        ).Sprite;

        var inactiveCannonRight = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right_inactive.png")
        ).Sprite;

        AlbireoCannonLeft = helper.Content.Ships.RegisterPart("AlbireoCannonLeft", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_left.png")
            ).Sprite,
            DisabledSprite = inactiveCannonLeft
        });

        AlbireoCannonRight = helper.Content.Ships.RegisterPart("AlbireoCannonRight", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/albireo_cannon_right.png")
            ).Sprite,
            DisabledSprite = inactiveCannonRight
        });

        AlbireoMissileBayLeft = helper.Content.Ships.RegisterPart("AlbireoMissileBayLeft", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_left.png")
            ).Sprite,
            DisabledSprite = inactiveMissileBaySprite
        });

        AlbireoMissileBayRight = helper.Content.Ships.RegisterPart("AlbireoMissileBayRight", new () 
        {
            Sprite = helper.Content.Sprites.RegisterSprite(
                package.PackageRoot.GetRelativeFile("assets/parts/albireo_missilebay_right.png")
            ).Sprite,
            DisabledSprite = inactiveMissileBaySprite
        });

            
        ShipEntry = helper.Content.Ships.RegisterShip("Albireo", new () 
        {
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "description"]).Localize,
            UnderChassisSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/parts/albireo_chassis.png")).Sprite,
            Ship = new() 
            {
                ship = new()
                {
                    hull = 10,
                    hullMax = 10,
                    shieldMaxBase = 4,
                    parts = [
                        new Part()
                        {
                            type = PType.missiles,
                            skin = AlbireoMissileBayLeft.UniqueName,
                            key = "left_missile",
                        },
                        new Part()
                        {
                            type = PType.cannon,
                            skin = AlbireoCannonLeft.UniqueName,
                            key = "left_cannon",
                        },
                        new Part()
                        {
                            type = PType.cockpit,
                            skin = helper.Content.Ships.RegisterPart("AlbireoCockpit", new() 
                            {
                                Sprite = helper.Content.Sprites.RegisterSprite(
                                    package.PackageRoot.GetRelativeFile("assets/parts/albireo_cockpit.png")
                                ).Sprite
                            }).UniqueName
                        },
                        new Part()
                        {
                            type = PType.cannon,
                            skin = AlbireoCannonRight.UniqueName,
                            key = "right_cannon",
                        },
                        new Part()
                        {
                            type = PType.missiles,
                            skin = AlbireoMissileBayRight.UniqueName,
                            key = "right_missile",
                        }
                    ]
                },
                artifacts = [new ShieldPrep(), new DoubleStar()],
                cards = [
                    new DodgeColorless(),
                    new CannonColorless(),
                    new BasicShieldColorless(),
                    new BasicSpacer()
                ]
            },
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DrawBG)),
            postfix: new HarmonyMethod(Combat_DrawBG_Postfix)
        );

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.RenderWalls)),
            postfix: new HarmonyMethod(Combat_RenderWalls_Postfix)
        );
    }

    internal static void Combat_RenderWalls_Postfix(Combat __instance, G g) 
    {
        if (!g.state.EnumerateAllArtifacts().Any(x => x is DoubleStar))
        {
            return;
        }

        const double AllY = 19.0;

        bool drawIcons = __instance.introTimer > 1.0;
        var rect = new Rect?(new Rect() + Combat.arenaPos + __instance.GetCamOffset());
        g.Push(rect: rect);
        {
            Box box = g.Push(new UIKey(AlbireoAUK), new Rect?(new Rect(0.0, 40.0, 16.0, 50.0) + new Vec((16 * 7) - 1, AllY)));
            Vec boxPos = box.rect.xy;

            Color starBlueColor = new Color(0.01, 0.05, 0.5, 1.0).gain(15);
            if (drawIcons && box.IsHover())
            {
                var albireoA = new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::Albireo::A") 
                {
                    Title = ModEntry.Instance.Localizations.Localize(["ship", "Albireo", "tooltip", "AlbireoA", "title"]),
                    TitleColor = starBlueColor,
                    Description = ModEntry.Instance.Localizations.Localize(["ship", "Albireo", "tooltip", "AlbireoA", "description"])
                };
                g.tooltips.Add(boxPos + new Vec(17.0, 0.0), albireoA);
            }

            Draw.Rect(boxPos.x - 1.0 + 9, -1000.0, 1, 2000.0, starBlueColor with { a = 0.6 });

            if (drawIcons)
            {
                Color? color = box.IsHover() ? null : new Color(1.0, 1.0, 1.0, 0.3);

                Draw.Sprite(AlbireoAIcon.Sprite, boxPos.x, boxPos.y + 17.0, color: color);
            }
            g.Pop();
        }
        g.Pop();

        rect = new Rect?(new Rect() + Combat.arenaPos + __instance.GetCamOffset());
        g.Push(rect: rect);
        {
            Box box = g.Push(new UIKey(AlbireoBUK), new Rect?(new Rect(0.0, 40.0, 16.0, 50.0) + new Vec((16 * 11) - 1, AllY)));
            Vec boxPos = box.rect.xy;

            Color starOrangeColor = new Color(0.5, 0.15, 0.01, 1.0).gain(10);
            if (drawIcons && box.IsHover())
            {
                var albireoA = new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::Albireo::B") 
                {
                    Title = ModEntry.Instance.Localizations.Localize(["ship", "Albireo", "tooltip", "AlbireoB", "title"]),
                    TitleColor = starOrangeColor,
                    Description = ModEntry.Instance.Localizations.Localize(["ship", "Albireo", "tooltip", "AlbireoB", "description"])
                };
                g.tooltips.Add(boxPos + new Vec(17.0, 0.0), albireoA);
            }

            Draw.Rect(boxPos.x - 1.0 + 9, -1000.0, 1, 2000.0, starOrangeColor with { a = 0.6 });

            if (drawIcons)
            {
                Color? color = box.IsHover() ? null : new Color(1.0, 1.0, 1.0, 0.3);

                Draw.Sprite(AlbireoBIcon.Sprite, boxPos.x, boxPos.y + 17.0, color: color);
            }
            g.Pop();
        }
        g.Pop();
    }

    internal static void Combat_DrawBG_Postfix(Combat __instance, G g)
    {
        if (__instance.modifier is MBinaryStar)
        {
            return;
        }

        if (!g.state.EnumerateAllArtifacts().Any(x => x is DoubleStar))
        {
            return;
        }

        double t = g.state.map.age;
        int moveSpeed = g.settings.reduceMotion ? 0 : 48;

        var offset = new Vec(-__instance.camX * 16.0, t * moveSpeed);

		const double PI = Math.PI;
		Color leftColor = new Color(0.01, 0.05, 0.5, 1.0);
		Color rightColor = new Color(0.5, 0.15, 0.01, 1.0);
		MapBase map = g.state.map;

		offset.y *= 0.2;

		BGComponents.NormalStars(g, map.age, offset);
		BGComponents.RegularNebula(g, offset, rightColor.gain(0.5));

		Vec left = new Vec(100, 0);
		Vec right = new Vec(500, 250);

		left.x += offset.x * 0.2;
		right.x += offset.x * 0.2;
		double orbitAngle = PI * 2.0 + (486 * PI / 180);

		Vec off = new Vec(-29, 9);
		Vec star1Pos = right + off;
		Vec star2Pos = left + off;
		double star1Size = Math.Sin(orbitAngle);

        BGComponents.Star(star1Pos, rightColor.gain(3.0), 70.0 + 50.0 * star1Size, 0.5 + 0.5 * star1Size);
		BGComponents.Star(star2Pos, leftColor.gain(20.0), 70.0 + 50.0 * star1Size, 0.5 + 0.5 * star1Size);

		Glow.Draw(star1Pos, 500.0, rightColor.gain(5.0).gain(0.5 + 0.5 * Math.Cos(orbitAngle + PI)));
		Glow.Draw(star2Pos, 500.0, leftColor.gain(5.0).gain(0.5 + 0.5 * Math.Cos(orbitAngle + PI)));

		BGComponents.RegularGlowMono(g, offset + new Vec(0.0, map.age * 100.0), rightColor.gain(0.5), new Vec?(new Vec(0.4, 1.0)));
    }
}