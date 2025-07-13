using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class FrostCannon : Artifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "FrostCannon",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_FrostCannon.Sprite,
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "artifact", "FrostCannon", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Gliese", "artifact", "FrostCannon", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        state.ship.Add(Status.stunCharge, 3);
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return
        [
            new TTGlossary("status.stunCharge", ["3"]),
            new TTGlossary("action.stun", Array.Empty<object>()),
            .. StatusMeta.GetTooltips(ColdStatus.ColdEntry.Status, 1),
        ];
    }
}
