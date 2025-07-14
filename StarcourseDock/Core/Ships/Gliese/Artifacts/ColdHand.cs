using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class ColdHand : Artifact, IKokoroApi.IV2.IRedrawStatusApi.IHook
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "ColdHand",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new() { owner = Deck.colorless, pools = [ArtifactPool.Common] },
                Sprite = Sprites.artifacts_ColdHand.Sprite,
                Name = Localization.ship_Gliese_artifact_ColdHand_name(),
                Description = Localization.ship_Gliese_artifact_ColdHand_description(),
            }
        );

        ModEntry.Instance.KokoroAPI.V2.RedrawStatus.RegisterHook(new ColdHandHook(), 10);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return
        [
            .. StatusMeta.GetTooltips(ModEntry.Instance.KokoroAPI.V2.RedrawStatus.Status, 0),
            GlieseKit.GetFrozenTraitTooltip(),
        ];
    }

    private class ColdHandHook : IKokoroApi.IV2.IRedrawStatusApi.IHook
    {
        public bool PayForRedraw(IKokoroApi.IV2.IRedrawStatusApi.IHook.IPayForRedrawArgs args)
        {
            if (
                ModEntry.Instance.Helper.ModData.TryGetModData(
                    args.Card,
                    "FrozenCount",
                    out int frozenCount
                )
                && frozenCount > 3
            )
            {
                return true;
            }

            return false;
        }

        public bool? CanRedraw(IKokoroApi.IV2.IRedrawStatusApi.IHook.ICanRedrawArgs args)
        {
            var coldHand = args.State.GetArtifactFromColorless<ColdHand>();
            if (coldHand is null)
            {
                return null;
            }

            if (
                ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(
                    args.State,
                    args.Card,
                    GlieseKit.FrozenTrait
                )
                && ModEntry.Instance.Helper.ModData.TryGetModData(
                    args.Card,
                    "FrozenCount",
                    out int frozenCount
                )
                && frozenCount > 3
            )
            {
                return true;
            }

            return null;
        }
    }
}
