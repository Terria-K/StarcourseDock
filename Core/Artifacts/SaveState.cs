using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SaveState : Artifact, IRegisterable
{
	public bool allowRepairBroken;
    public List<Part>? tempParts;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("SaveState", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.SaveState.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SaveState", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SaveState", "description"]).Localize
		});
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
			Pulse();
        }
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new AResetShip
        {
            parts = tempParts
        });
    }
}