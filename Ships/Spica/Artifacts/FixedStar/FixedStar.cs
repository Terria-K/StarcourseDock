using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class FixedStar : Artifact, IRegisterable
{
    private List<Part>? tempParts;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
        var fixedStarSprite = helper.Content.Sprites.RegisterSprite(
            package.PackageRoot.GetRelativeFile("assets/artifacts/FixedStar.png")
        );
		helper.Content.Artifacts.RegisterArtifact("FixedStar", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = fixedStarSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "FixedStar", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "FixedStar", "description"]).Localize
		});

        ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterHook(new FixedStarHook(), 10);
        var evadeAction = ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterAction(new FixedStarAction(), 10);
        evadeAction.RegisterPaymentOption(new FixedStarPaymentOption());
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard { card = new Merge() }];
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        bool alreadyHasOne = false;
        foreach (var card in combat.hand)
        {
            if (card is Merge)
            {
                alreadyHasOne = true;
                break;
            }
        }

        if (!alreadyHasOne)
        {
            combat.Queue(new AAddCard
            {
                card = new Merge(),
                destination = CardDestination.Hand
            });
        }


        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
            return;
        }
        Reset(state);
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new AResetShip
        {
            parts = tempParts
        });
    }

    private static void Reset(State state)
    {
        int i = 0;
        int cannonIndex = 0;
        bool shouldContinue = true;
        foreach (var s in state.ship.parts)
        {
            if (s.key == "toRemove")
            {
                shouldContinue = false;
                break;
            }
            if (s.key == "closeToScaffold")
            {
                cannonIndex = i;
            }
            i += 1;
        }

        if (!shouldContinue)
        {
            return;
        }

        state.ship.parts.Insert(cannonIndex + 1, new Part() 
        {
            type = PType.empty,
            skin = SpicaShip.SpicaScaffold.UniqueName,
            key = "toRemove"
        });

        if (state.ship.IsTouchingRightWall(state))
        {
            state.ship.x -= 1;
            state.ship.xLerped -= 1;
        }
    }
}