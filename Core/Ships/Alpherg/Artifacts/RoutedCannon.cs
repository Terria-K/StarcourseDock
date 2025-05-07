using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class RoutedCannon : Artifact, IRegisterable
{
    public bool disabled;

    public static ISpriteEntry RoutedCannonSprite { get; set; } = null!;
    public static ISpriteEntry RoutedCannonInactiveSprite { get; set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        RoutedCannonSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannon.png")
        );

        RoutedCannonInactiveSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/RoutedCannonInactive.png")
        );

		helper.Content.Artifacts.RegisterArtifact("RoutedCannon", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = RoutedCannonSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "RoutedCannon", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "RoutedCannon", "description"]).Localize
		});
    }

    public override Spr GetSprite()
    {
        if (disabled)
        {
            return RoutedCannonInactiveSprite.Sprite;
        }
        return RoutedCannonSprite.Sprite;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (disabled)
        {
            disabled = false;
        }

        combat.Queue(new AModifyCannon() { active = false });
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        if (card is RerouteCannon rerouteCannon)
        {
            if (rerouteCannon.upgrade == Upgrade.B)
            {
                return;
            }
            disabled = true;
        }
    }
}