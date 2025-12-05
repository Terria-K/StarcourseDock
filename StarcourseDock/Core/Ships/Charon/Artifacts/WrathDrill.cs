using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class WrathDrill : Artifact, IRegisterable
{
    public int attacks;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "WrathDrill",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_WrathDrill.Sprite,
                Name = Localization.ship_Charon_artifact_WrathDrill_name(),
                Description = Localization.ship_Charon_artifact_WrathDrill_description()
            }

        );
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
            state.ship.Add(WrathChargeStatus.WrathCharge.Status, 1);
        }
    }
}