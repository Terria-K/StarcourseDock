using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FrostCannon : Artifact, IRegisterable
{
    public int count;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
		helper.Content.Artifacts.RegisterArtifact("FrostCannon", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.FrostCannon.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "FrostCannon", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "FrostCannon", "description"]).Localize
		});
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count = 0;
        int stunCharges = state.ship.Get(Status.stunCharge);
        if (stunCharges < 3)
        {
            state.ship.Add(Status.stunCharge, 3);
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
			new TTGlossary("status.stunCharge", ["3"]),
			new TTGlossary("status.lockdown"),
			new TTGlossary("action.stun", Array.Empty<object>()),
			new TTGlossary("action.stunShip"),
        ];
    }

    public override void OnEnemyGetHit(State state, Combat combat, Part? part)
    {
        if (part != null)
        {
            int stunStatus = state.ship.Get(Status.stunCharge);
            if (part.intent != null && stunStatus > 0)
            {
                count += 1;

                if (count >= 3)
                {
                    combat.otherShip.Add(Status.lockdown, 1);
                    count = 0;
                    combat.Queue(new AStunShip());
                }
            }
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }
}
