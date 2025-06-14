using System.Reflection;
using System.Reflection.Emit;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using Teuria.Utilities;

namespace Teuria.StarcourseDock;

internal class Piscium : Artifact, IRegisterable
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Alpherg", "artifact", "Piscium", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Alpherg", "artifact", "Piscium", "description"]
                    )
                    .Localize,
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
        if (aAttack != null)
        {
            return;
        }
        isRight = !isRight;
        combat.QueueImmediate(new ASwapScaffold() { isRight = isRight });
    }
}
