using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class DoublePolarity : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DoublePolarity",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_EnergyDoubler.Sprite,
                Name = Localization.ship_Albireo_artifact_DoublePolarity_name(),
                Description = Localization.ship_Albireo_artifact_DoublePolarity_description()
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        var deck = state.GetArtifactFromColorless<DoubleDeck>();

        if (deck is null)
        {
            return;
        }
        deck.maxCapacity = 5;

        if (deck.count == 2)
        {
            return;
        }

        if (deck.count == 1)
        {
            deck.count += 1;
        }

        if (deck.count == 3)
        {
            deck.count -= 1;
        }
    }

    public override List<Tooltip>? GetExtraTooltips() => [Polarity.GetTooltip()];
}
