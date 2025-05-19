using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class HeatShield : Artifact, IRegisterable
{
    public int currentHeatCounter;
    public int heatDifference;
    public bool hit;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("HeatShield", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.EventOnly],
                unremovable = true,
            },
            Sprite = Sprites.HeatShield.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "artifact", "HeatShield", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "artifact", "HeatShield", "description"]).Localize
        });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new AStatus() { statusAmount = 2, status = Status.shield });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        currentHeatCounter = 0;
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        heatDifference = state.ship.Get(Status.heat);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            new TTGlossary("status.shield", ["2"]),
            new TTGlossary("status.heat", ["3"])
        ];
    }

    public override void OnPlayerTakeNormalDamage(State state, Combat combat, int rawAmount, Part? part)
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
        if (heat <= heatDifference)
        {
            return;
        }

        heatDifference = heat;

        currentHeatCounter += 1;

        if (currentHeatCounter >= 2)
        {
            state.ship.Add(Status.shield, 2);
            Pulse();
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return currentHeatCounter;
    }
}