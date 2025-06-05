using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class FixedStar : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "FixedStar",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_FixedStar.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Spica", "artifact", "FixedStar", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Spica", "artifact", "FixedStar", "description"]
                    )
                    .Localize,
            }
        );

        ModEntry.Instance.KokoroAPI.V2.EvadeHook.RegisterHook(new FixedStarHook(), 10);
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

            if (!state.HasArtifactFromColorless<FixedStar>())
            {
                return true;
            }

            if (!state.ship.IsPartExists(PType.cannon))
            {
                return false;
            }

            if (state.HasArtifactFromColorless<TinyWormhole>())
            {
                return true;
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
