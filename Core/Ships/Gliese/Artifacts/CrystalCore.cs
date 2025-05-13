using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class CrystalCore : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("CrystalCore", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.CrystalCore.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "CrystalCore", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Gliese", "artifact", "CrystalCore", "description"]).Localize
		});
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            return;
        }

        combat.Queue(new ARemoveAllBrokenPart());
        combat.Queue(new AShuffleShip() { targetPlayer = true });
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new ARepairAllBrokenPart());
    }

    public override void OnPlayerTakeNormalDamage(State state, Combat combat, int rawAmount, Part? part)
    {
        if (part != null && part.key == "portal")
        {
            state.ship.InsertPart(state, "portal", "portal", false, new Part() 
            {
                type = PType.special,
                skin = GlieseShip.GlieseCrystal2.UniqueName,
                stunModifier = PStunMod.breakable,
                key = "crystal2::StarcourseDock"
            });
        }
    }
}
