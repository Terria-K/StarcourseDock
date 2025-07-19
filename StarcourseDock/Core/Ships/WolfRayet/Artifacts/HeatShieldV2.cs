using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class HeatShieldV2 : Artifact, IRegisterable
{
    public int currentHeatCounter;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "HeatShieldV2",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_VolcanicShield.Sprite,
                Name = Localization.ship_WolfRayet_artifact_HeatShieldV2_name(),
                Description = Localization.ship_WolfRayet_artifact_HeatShieldV2_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new HeatShield().Key() });

        state.ship.hullMax = Math.Max(1, state.ship.hullMax - 5);
        if (state.ship.hull > state.ship.hullMax)
        {
            state.ship.hull = state.ship.hullMax;
        }
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            combat.Queue(
                new AStatus()
                {
                    statusAmount = 3,
                    status = Status.shield,
                    targetPlayer = true,
                }
            );
            Pulse();
        }
        currentHeatCounter = 0;
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        int heatMin = 3;
        if (MG.inst.g.state.ship != null)
        {
            heatMin = MG.inst.g.state.ship.heatTrigger;
        }
        return [new TTGlossary("status.shield", ["3"]), new TTGlossary("status.heat", [heatMin])];
    }

    public override int? GetDisplayNumber(State s)
    {
        return currentHeatCounter;
    }
}
