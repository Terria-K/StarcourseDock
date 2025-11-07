using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GravitationalPullV2 : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "GravitationalPullV2",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_SmartPull.Sprite,
                Name = Localization.ship_Io_artifact_GravitationalPullV2_name(),
                Description = Localization.ship_Io_artifact_GravitationalPullV2_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new GravitationalPull().Key() });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard() { card = new Release() { upgrade = Upgrade.A } }
        ];
    }
}

