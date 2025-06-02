using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class HeatShieldV2 : Artifact, IRegisterable
{
    public int currentHeatCounter;
    public int oldHeat;
    public bool hit;

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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "HeatShieldV2", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "HeatShieldV2", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new HeatShield().Key() });
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

    public override void OnTurnEnd(State state, Combat combat)
    {
        oldHeat = state.ship.Get(Status.heat);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTGlossary("status.shield", ["3"]), new TTGlossary("status.heat", ["3"])];
    }

    public override void OnPlayerTakeNormalDamage(
        State state,
        Combat combat,
        int rawAmount,
        Part? part
    )
    {
        hit = true;
    }

    public override void OnEnemyAttack(State state, Combat combat)
    {
        if (!hit)
        {
            return;
        }

        hit = false;

        var heat = state.ship.Get(Status.heat);
        if (heat <= oldHeat)
        {
            return;
        }

        oldHeat = heat;

        currentHeatCounter += heat - oldHeat;

        if (currentHeatCounter >= 2)
        {
            currentHeatCounter = 0;
            state.ship.Add(Status.shield, 3);
            Pulse();
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return currentHeatCounter;
    }
}
