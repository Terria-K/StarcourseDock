using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;
using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class FixedStar : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("FixedStar", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Deck.colorless,
				pools = [ArtifactPool.EventOnly],
				unremovable = true,
			},
			Sprite = Sprites.FixedStar.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "FixedStar", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Spica", "artifact", "FixedStar", "description"]).Localize
		});

        ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterHook(new FixedStarHook(), 10);

        ModEntry.Instance.Harmony.Patch(
            AccessTools.DeclaredMethod(typeof(AMove), nameof(AMove.Begin)),
            prefix: new HarmonyMethod(AMove_Begin_Prefix)
        );
    }

    public static bool AMove_Begin_Prefix(AMove __instance, State s, Combat c)
    {
        if (s.EnumerateAllArtifacts().Any(x => x is FixedStar) && __instance.targetPlayer && __instance.fromEvade)
        {
            __instance.timer = 0f;
            c.QueueImmediate(new ACannonMove() { dir = __instance.dir, ignoreHermes = __instance.ignoreHermes, preferRightWhenZero = __instance.preferRightWhenZero });
            return false;
        }

        return true;
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard { card = new Shrink() }];
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        bool alreadyHasOne = false;
        foreach (var card in combat.hand)
        {
            if (card is Shrink)
            {
                alreadyHasOne = true;
                break;
            }
        }

        if (!alreadyHasOne)
        {
            combat.Queue(new AAddCard
            {
                card = new Shrink(),
                destination = CardDestination.Hand
            });
        }


        if (combat.turn == 1)
        {
            return;
        }
        Reset(state);
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

    private class FixedStarHook : IHook
    {
        public bool IsEvadeActionEnabled(IHook.IIsEvadeActionEnabledArgs args) 
        {
            var state = args.State;

            if (!state.EnumerateAllArtifacts().Any(x => x is FixedStar))
            {
                return true;
            }

            int dir = (int)args.Direction;
            int cannonIndex = 0;
            int i = 0;
            int length = state.ship.parts.Count;
            foreach (var s in state.ship.parts)
            {
                if (s.type == PType.cannon)
                {
                    cannonIndex = i;
                    break;
                }
                i += 1;
            }
            if (dir <= -1 && cannonIndex <= 1)
            {
                return false;
            }

            if (dir >= 1 && cannonIndex >= length - 2)
            {
                return false;
            }

            return true;
        }
    }
}