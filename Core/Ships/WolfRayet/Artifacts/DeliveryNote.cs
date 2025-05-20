using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class DeliveryNote : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("DeliveryNote", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.EventOnly],
                unremovable = true,
            },
            Sprite = Sprites.DeliveryNote.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "artifact", "DeliveryNote", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "WolfRayet", "artifact", "DeliveryNote", "description"]).Localize
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        for (int i = 0; i < state.ship.parts.Count; i++)
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
