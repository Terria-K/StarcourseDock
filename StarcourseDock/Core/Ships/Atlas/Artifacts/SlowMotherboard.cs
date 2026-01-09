using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SlowMotherboard : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SlowMotherboard",
            new()
            {
                Name = Localization.ship_Atlas_artifact_SlowMotherboard_name(),
                Description = Localization.ship_Atlas_artifact_SlowMotherboard_description(),
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true
                },
                Sprite = Sprites.artifacts_SlowMotherboard.Sprite
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new FrailMotherboard().Key() });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(new AAddCard() { card = new EngineMaintenance() { upgrade = Upgrade.A}, destination = CardDestination.Deck });
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTCard() { card = new EngineMaintenance() { upgrade = Upgrade.A } }
        ];
    }
}
