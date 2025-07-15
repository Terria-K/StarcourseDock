using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal class ShrinkMechanismV2 : Artifact, IRegisterable
{
    public List<Part>? tempParts;
    public List<Part>? leftParts;
    public List<Part>? rightParts;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "ShrinkMechanismV2",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_ShrinkMechanismV2.Sprite,
                Name = Localization.ship_Spica_artifact_ShrinkMechanismV2_name(),
                Description = Localization.ship_Spica_artifact_ShrinkMechanismV2_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new ShrinkMechanism().Key() });
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard { card = new Shrink() { upgrade = Upgrade.A } }];
    }

    public override void OnPlayerTakeNormalDamage(
        State state,
        Combat combat,
        int rawAmount,
        Part? part
    )
    {
        if (part == null || part.type != PType.wing)
        {
            return;
        }
        leftParts ??= new List<Part>();
        rightParts ??= new List<Part>();

        switch (part.key)
        {
            case "rightwing":
                int? rx = state.ship.GetLocalXOfPart("rightwing");
                if (rx != null)
                {
                    int v = rx.Value;
                    Part? p = state.ship.GetPartAtLocalX(v - 1);
                    if (p != null && p.type != PType.wing)
                    {
                        rightParts.Add(p);
                        state.ship.RemoveParts("leftwing", [p.key!]);
                    }
                }
                break;
            case "leftwing":
                int? lx = state.ship.GetLocalXOfPart("leftwing");
                if (lx != null)
                {
                    int v = lx.Value;
                    Part? p = state.ship.GetPartAtLocalX(v + 1);
                    if (p != null && p.type != PType.wing)
                    {
                        leftParts.Add(p);
                        state.ship.RemoveParts("rightwing", [p.key!]);
                    }
                }
                break;
        }
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (!combat.HasCardOnHand<Shrink>())
        {
            combat.Queue(
                new AAddCard
                {
                    card = new Shrink() { upgrade = Upgrade.A },
                    destination = CardDestination.Hand,
                }
            );
        }

        if (combat.turn == 1)
        {
            tempParts = [.. state.ship.parts];
            return;
        }

        Reset(state);
    }

    private void Reset(State state)
    {
        if (leftParts != null)
        {
            int index = state.ship.FindPartIndex("leftwing");
            state.ship.InsertParts(state, index, index, true, leftParts, true);
            leftParts.Clear();
        }

        if (rightParts != null)
        {
            int index = state.ship.FindPartIndex("rightwing");
            List<Part> partReversed = [.. rightParts];
            partReversed.Reverse();
            state.ship.InsertParts(
                state,
                index,
                index,
                false,
                partReversed,
                true
            );
            rightParts.Clear();
        }
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Queue(new AResetShip { parts = tempParts });
    }
}
