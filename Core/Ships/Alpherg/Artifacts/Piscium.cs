using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class Piscium : Artifact, IRegisterable
{
    public bool isRight;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
        var pisciumSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/Piscium.png")
        );
		helper.Content.Artifacts.RegisterArtifact("Piscium", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = pisciumSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "Piscium", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Alpherg", "artifact", "Piscium", "description"]).Localize
		});
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            isRight = false;
            return;
        }
        isRight = !isRight;
        combat.Queue(new ASwapScaffold() { isRight = isRight });
    }

    public override void OnCombatEnd(State state)
    {
        if (isRight)
        {
            isRight = false;
            state.rewardsQueue.Queue(new ASwapScaffold() { isRight = false });
        }
    }
}
