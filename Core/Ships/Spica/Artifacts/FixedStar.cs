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
        if (s.HasArtifact<FixedStar>() && __instance.targetPlayer && __instance.fromEvade)
        {
            __instance.timer = 0f;
            c.QueueImmediate(new ACannonMove() { dir = __instance.dir, ignoreHermes = __instance.ignoreHermes, preferRightWhenZero = __instance.preferRightWhenZero });
            return false;
        }

        return true;
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTGlossary("status.evade")];
    }

    private class FixedStarHook : IHook
    {
        public bool IsEvadeActionEnabled(IHook.IIsEvadeActionEnabledArgs args) 
        {
            var state = args.State;

            if (!state.HasArtifact<FixedStar>() || state.HasArtifact<TinyWormhole>())
            {
                return true;
            }

            if (!state.ship.HasPartType(PType.cannon))
            {
                return false;
            }

            int dir = (int)args.Direction;
            int cannonIndex = state.ship.FindPartIndex(PType.cannon);
            int length = state.ship.parts.Count;
            
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
