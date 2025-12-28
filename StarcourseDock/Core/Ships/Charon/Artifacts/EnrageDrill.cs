using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class EnrageDrill : Artifact, IRegisterable
{
    public int attacks;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "EnrageDrill",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_EnrageDrill.Sprite,
                Name = Localization.ship_Charon_artifact_EnrageDrill_name(),
                Description = Localization.ship_Charon_artifact_EnrageDrill_description()
            }

        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new WrathDrill().Key() });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        attacks = 0;
    }

    public override int? GetDisplayNumber(State s)
    {
        return attacks;
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        if (!combat.isPlayerTurn)
        {
            return;
        }

        attacks += 1;

        if (attacks >= 2)
        {
            attacks = 0;
            state.ship.Add(WrathChargeStatus.WrathCharge.Status, 2);
            Pulse();
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            WrathChargeStatus.GetTooltip(1, 2, 1)
        ];
    }
}
