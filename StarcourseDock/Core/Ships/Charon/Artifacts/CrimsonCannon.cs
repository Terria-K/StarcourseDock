using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class CrimsonCannon : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "CrimsonCannon",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss]
                },
                Sprite = Sprites.artifacts_CrimsonCannon.Sprite,
                Name = Localization.ship_Charon_artifact_CrimsonCannon_name(),
                Description = Localization.ship_Charon_artifact_CrimsonCannon_description()
            }
        );
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        var list = new List<AFragile>();

        int x = 0;
        foreach (var part in state.ship.parts)
        {
            if (part.type != PType.cannon)
            {
                x += 1;
                continue;
            }

            list.Add(new AFragile()
            {
                targetPlayer = true,
                worldX = state.ship.x + x,
                justTheActiveOverride = true,
                artifactPulse = Key()
            });
            x += 1;
        }

        combat.QueueImmediate(list);
    }

    public override void OnReceiveArtifact(State state)
    {
        state.ship.baseEnergy += 2;
    }

    public override void OnRemoveArtifact(State state)
    {
        state.ship.baseEnergy -= 2;
    }
}
