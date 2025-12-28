using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class DoubleCard : DoubleDeck, IRegisterable
{
    public static new void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DoubleCard",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_DoubleDeck.Sprite,
                Name = Localization.customRunOptions_DoubleCard_artifact_DoubleCard_name(),
                Description = Localization.customRunOptions_DoubleCard_artifact_DoubleCard_description()
            }
        );
    }
}
