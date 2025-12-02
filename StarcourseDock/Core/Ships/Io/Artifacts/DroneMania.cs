using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class DroneMania : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DroneMania",
            new() 
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_DroneMania.Sprite,
                Name = Localization.ship_Io_artifact_DroneMania_name(),
                Description = Localization.ship_Io_artifact_DroneMania_description(),
            }
        );
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(
            new AAddCard { card = new BurstDrone(), destination = CardDestination.Hand });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard() { card = new BurstDrone() }
        ];
    }
}

