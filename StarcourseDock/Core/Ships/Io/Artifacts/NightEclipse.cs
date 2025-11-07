using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class NightEclipse : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "NightEclipse",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_NightEclipse.Sprite,
                Name = Localization.ship_Io_artifact_NightEclipse_name(),
                Description = Localization.ship_Io_artifact_NightEclipse_description(),
            }
        );
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.QueueImmediate(
            new AAddCard { card = new JupiterEclipse(), destination = CardDestination.Hand });
    }

    public override List<Tooltip> GetExtraTooltips()
    {
        return [
            new TTCard() { card = new JupiterEclipse() }
        ];
    }
}

