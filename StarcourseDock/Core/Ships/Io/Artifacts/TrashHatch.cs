using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class TrashHatch : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "TrashHatch",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_TrashHatch.Sprite,
                Name = Localization.ship_Io_artifact_TrashHatch_name(),
                Description = Localization.ship_Io_artifact_TrashHatch_description(),
            }
        );
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard() { card = new Release() }
        ];
    }
}

