using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class Piscium : Artifact, IRegisterable
{
    public bool isRight;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "Piscium",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_Piscium.Sprite,
                Name = Localization.ship_Alpherg_artifact_Piscium_name(),
                Description = Localization.ship_Alpherg_artifact_Piscium_description(),
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            combat.Queue(new ASwapScaffold() { isRight = isRight });
        }
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.QueueImmediate(new ASwapScaffold() { isRight = isRight });
    }

    public static AAttack? aAttack;

    public override void OnTurnEnd(State state, Combat combat)
    {
        aAttack = null;
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        if (aAttack != null && aAttack.multiCannonVolley)
        {
            return;
        }

        isRight = !isRight;

        int idx = combat.cardActions.FindIndex(x => x.GetType() == typeof(AJupiterShoot));
        if (idx == -1)
        {
            combat.QueueImmediate(new ASwapScaffold() { isRight = isRight });
            return;
        }

        combat.cardActions.Insert(idx, new ASwapScaffold() { isRight = isRight });
    }
}
