using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class DeliveryNote : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DeliveryNote",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_DeliveryNote.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "DeliveryNote", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "WolfRayet", "artifact", "DeliveryNote", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        int lastIndex = state.ship.parts.Count - 1;
        for (int i = lastIndex; i >= 0; i--)
        {
            var part = state.ship.parts[i];
            if (part.type == PType.empty)
            {
                combat.QueueImmediate(new AAddMissile() { x = i, targetPlayer = true });
                part.key = null;
            }
        }
        Pulse();
    }

    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.QueueImmediate(new ADisableAllMissiles());
    }
}
