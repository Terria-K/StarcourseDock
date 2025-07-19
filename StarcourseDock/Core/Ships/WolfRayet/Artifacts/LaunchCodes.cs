using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class LaunchCodes : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "LaunchCodes",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common],
                },
                Sprite = Sprites.artifacts_LaunchCodes.Sprite,
                Name = Localization.ship_WolfRayet_artifact_LaunchCodes_name(),
                Description = Localization.ship_WolfRayet_artifact_LaunchCodes_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state.GetCurrentQueue().QueueImmediate(new AAddCard() { card = new LaunchOverride(), destination = CardDestination.Deck });
        state.GetCurrentQueue().QueueImmediate(new AAddCard() { card = new FalseLaunch(), destination = CardDestination.Deck });
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new LaunchOverride() },
            new TTCard() { card = new FalseLaunch() }
        ];
    }
}
