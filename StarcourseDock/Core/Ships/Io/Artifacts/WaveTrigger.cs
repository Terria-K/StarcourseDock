using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class WaveTrigger : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "WaveInterferance",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Common],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_WaveTrigger.Sprite,
                Name = Localization.ship_Io_artifact_WaveTrigger_name(),
                Description = Localization.ship_Io_artifact_WaveTrigger_description(),
            }
        );
    }

    public override void OnPlayerDodgeHit(State state, Combat combat)
    {
        var aattack = AAttack_Global_Patches.Global_AAttack;
        if (aattack is null)
        {
            return;
        }

        if (aattack.fromDroneX is { } fromDroneX)
        {
            var part = state.ship.GetPartAtWorldX(fromDroneX);
            if (part is null)
            {
                return;
            }

            if (part.type == PType.empty)
            {
                combat.QueueImmediate(new AAttack() { damage = Card.GetActualDamage(state, 1, false) });
                Pulse();
            }

            return;
        }

        if (aattack.fromX is { } fromX)
        {
            var part = state.ship.GetPartAtWorldX(combat.otherShip.x + fromX);
            if (part is null)
            {
                return;
            }

            if (part.type == PType.empty)
            {
                combat.QueueImmediate(new AAttack() { damage = Card.GetActualDamage(state, 1, false) });
                Pulse();
            }
        }
    }
}


