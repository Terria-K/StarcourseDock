using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class SiriusSubwoofer : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("SiriusSubwoofer", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.Common],
                unremovable = true,
            },
            Sprite = Sprites.SiriusSubwoofer.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusSubwoofer", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Sirius", "artifact", "SiriusSubwoofer", "description"]).Localize
        });
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
