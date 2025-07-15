using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SiriusSubwoofer : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SiriusSubwoofer",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new() { owner = Deck.colorless, pools = [ArtifactPool.Common] },
                Sprite = Sprites.artifacts_SiriusSubwoofer.Sprite,
                Name = Localization.ship_Sirius_artifact_SiriusSubwoofer_name(),
                Description = Localization.ship_Sirius_artifact_SiriusSubwoofer_description(),
            }
        );
    }

    public override int ModifyBaseJupiterDroneDamage(State state, Combat? combat, StuffBase midrow)
    {
        if (midrow is not SiriusDrone)
        {
            return 0;
        }

        int partX = state.ship.parts.FindIndex(p => p.type == PType.comms);
        if (partX >= 0 && midrow.x == partX + state.ship.x)
        {
            return 1;
        }

        return 0;
    }
}
