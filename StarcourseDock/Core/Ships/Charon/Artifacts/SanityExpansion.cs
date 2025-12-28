using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class SanityExpansion : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SanityExpansion",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common]
                },
                Sprite = Sprites.artifacts_SanityExpansion.Sprite,
                Name = Localization.ship_Charon_artifact_SanityExpansion_name(),
                Description = Localization.ship_Charon_artifact_SanityExpansion_description()
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            WrathChargeStatus.GetTooltip(1, 2, 1)
        ];
    }
}
