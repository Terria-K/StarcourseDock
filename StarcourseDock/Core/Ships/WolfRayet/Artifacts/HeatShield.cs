using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class HeatShield : Artifact, IRegisterable
{
    public int currentHeatCounter;
    public int oldHeat;
    public bool hit;
    public bool goalAchieved;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "HeatShield",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_HeatShield.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "HeatShield", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "HeatShield", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            combat.Queue(
                new AStatus()
                {
                    statusAmount = 2,
                    status = Status.shield,
                    targetPlayer = true,
                }
            );
            Pulse();
        }
        currentHeatCounter = 0;
        goalAchieved = false;
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        oldHeat = state.ship.Get(Status.heat);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTGlossary("status.shield", ["2"]), new TTGlossary("status.heat", ["3"])];
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
        if (goalAchieved)
        {
            return;
        }
        var heat = state.ship.Get(Status.heat);
        if (heat <= oldHeat)
        {
            return;
        }

        oldHeat = heat;

        currentHeatCounter += heat - oldHeat;

        if (currentHeatCounter >= 2)
        {
            goalAchieved = true;
            state.ship.Add(Status.shield, 2);
            Pulse();
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return currentHeatCounter;
    }

    public override Spr GetSprite()
    {
        if (goalAchieved)
        {
            return Sprites.artifacts_HeatShieldInactive.Sprite;
        }

        return Sprites.artifacts_HeatShield.Sprite;
    }
}
