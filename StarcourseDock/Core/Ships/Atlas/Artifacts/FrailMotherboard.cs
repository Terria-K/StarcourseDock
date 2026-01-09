using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FrailMotherboard : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "FrailMotherboard",
            new()
            {
                Name = Localization.ship_Atlas_artifact_FrailMotherboard_name(),
                Description = Localization.ship_Atlas_artifact_FrailMotherboard_description(),
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true
                },
                Sprite = Sprites.artifacts_FrailMotherboard.Sprite
            }
        );
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AAddCard() { card = new EngineMaintenance(), destination = CardDestination.Deck });
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new EngineMaintenance() }
        ];
    }
}
