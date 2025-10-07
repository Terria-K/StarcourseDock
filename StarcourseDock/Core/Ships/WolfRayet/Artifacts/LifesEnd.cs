using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class LifesEnd : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "LifesEnd",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_LifesEnd.Sprite,
                Name = Localization.ship_WolfRayet_artifact_LifesEnd_name(),
                Description = Localization.ship_WolfRayet_artifact_LifesEnd_description(),
            }
        );
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        var state = MG.inst.g.state;
        string hAmount;

        if (state is null || state.map is null)
        {
            hAmount = "30/40/50";
        }
        else 
        {
            hAmount = GetMaxHull(state).ToString();
        }

        return [
            new TTText(Localization.Str_ship_WolfRayet_artifact_LifesEnd_extra_description(hAmount))
        ];
    }

    private static int GetMaxHull(State s)
    {
        var type = s.map.GetType();
        if (type == typeof(MapFirst))
        {
            return 50;
        }
        else if (type == typeof(MapLawless))
        {
            return 40;
        }
        else
        {
            return 30;
        }
    }

    public override void OnReceiveArtifact(State state)
    {
        int finalHealAmt = 0;

        int amountHeal = GetMaxHull(state);

        state.ship.hullMax += amountHeal;
        foreach (Artifact r in state.EnumerateAllArtifacts())
        {
            finalHealAmt += r.ModifyHealAmount(amountHeal, state, true);
        }
        state.ship.Heal(finalHealAmt + amountHeal);


        state.ship.shieldMaxBase = 0;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if ((combat.turn & 1) == 1)
        {
            state.ship.Add(Status.corrode, 1);
        }
    }
}

