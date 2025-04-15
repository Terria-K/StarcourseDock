using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class DoubleStar : Artifact, IRegisterable
{
    public bool binaryStarDetected = false;
    public static ISpriteEntry DoubleStarSprite { get; internal set; } = null!;
    public static ISpriteEntry DoubleStarInactiveSprite { get; internal set; } = null!;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
        DoubleStarSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStar.png")
        );
        DoubleStarInactiveSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/DoubleStarInactive.png")
        );
		helper.Content.Artifacts.RegisterArtifact("DoubleStar", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = DoubleStarSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "artifact", "DoubleStar", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "artifact", "DoubleStar", "description"]).Localize
		});

        var evadeAction = ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterAction(new DoubleStarAction(), 10);
        evadeAction.RegisterPaymentOption(new DoubleStarPaymentOption());
    }

    public override Spr GetSprite()
    {
        if (binaryStarDetected)
        {
            return DoubleStarInactiveSprite.Sprite;
        }
        return DoubleStarSprite.Sprite;
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new ACheckParts());
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        binaryStarDetected = combat.modifier is MBinaryStar;
        if (state.ship.x > 8)
        {
			combat.QueueImmediate(new AEnergy() 
            {
                changeAmount = -1
            });
        }
        else if (state.ship.x < 6)
        {
			combat.QueueImmediate(new AEnergy() 
            {
                changeAmount = 1
            });
        }
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        if (binaryStarDetected)
        {
            return;
        }

        combat.Queue(new ACheckParts());
        if (state.ship.x > 8)
        {
            Pulse();
            combat.Queue(new ADoubler() 
            {
                card = card
            });
        }
    }

    public override bool? ModifyAttacksToStun(State state, Combat? combat)
    {
        if (binaryStarDetected)
        {
            return false;
        }
        return state.ship.x < 6;
    }
}