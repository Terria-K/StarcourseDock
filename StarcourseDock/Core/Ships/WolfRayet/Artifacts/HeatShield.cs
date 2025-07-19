using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class HeatShield : Artifact, IRegisterable
{
    public int currentHeatCounter;
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
                Name = Localization.ship_WolfRayet_artifact_HeatShield_name(),
                Description = Localization.ship_WolfRayet_artifact_HeatShield_description(),
            }
        );

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(new HeatStatusHook());
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

    public override List<Tooltip>? GetExtraTooltips()
    {
        int heatMin = 3;
        if (MG.inst.g.state.ship != null)
        {
            heatMin = MG.inst.g.state.ship.heatTrigger;
        }
        return [new TTGlossary("status.shield", ["2"]), new TTGlossary("status.heat", [heatMin])];
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

file sealed class HeatStatusHook : IKokoroApi.IV2.IStatusLogicApi.IHook
{
    public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
    {
        if (args.Status != Status.heat || args.Combat.isPlayerTurn)
        {
            goto EXIT;
        }

        if (args.Ship != args.State.ship)
        {
            goto EXIT;
        }

        // heatShield
        var heatShield = args.State.GetArtifactFromColorless<HeatShield>();

        if (heatShield is not null)
        {
            if (heatShield.goalAchieved)
            {
                goto EXIT;
            }
            int heat = args.NewAmount - args.OldAmount;
            if (heat <= 0)
            {
                goto EXIT;
            }
            heatShield.currentHeatCounter += args.NewAmount - args.OldAmount;

            if (heatShield.currentHeatCounter >= 2)
            {
                heatShield.goalAchieved = true;
                args.State.ship.Add(Status.shield, 2);
                heatShield.Pulse();
            }

            goto EXIT;
        }
        var heatShieldV2 = args.State.GetArtifactFromColorless<HeatShieldV2>();

        if (heatShieldV2 is not null)
        {
            int heat = args.NewAmount - args.OldAmount;
            if (heat <= 0)
            {
                goto EXIT;
            }
            heatShieldV2.currentHeatCounter += args.NewAmount - args.OldAmount;

            if (heatShieldV2.currentHeatCounter >= 2)
            {
                heatShieldV2.currentHeatCounter = 0;
                args.State.ship.Add(Status.shield, 3);
                heatShieldV2.Pulse();
            }
        }

        EXIT:
        return args.NewAmount;
    }
}