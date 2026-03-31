using Nanoray.PluginManager;
using Nickel;

namespace Teuria.AttackDrone;

internal sealed class DroneMkII : Artifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DroneMkII",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifacts", "DroneMkII", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifacts", "DroneMkII", "description"]).Localize,
                ArtifactType = typeof(DroneMkII),
                Sprite = helper.Content.Sprites.RegisterSprite(
                    "DroneMkII",
                    package.PackageRoot.GetRelativeFile("assets/DroneMkII.png")
                ).Sprite,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true
                }
            }
        );
    }

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        return 1;
    }

    public override void OnReceiveArtifact(State state)
    {
        foreach (var p in state.ship.parts)
        {
            if (p.type == PType.cannon && p.skin == AttackDroneShip.AttackDroneSkin.UniqueName)
            {
                p.skin = AttackDroneShip.AttackDroneIISkin.UniqueName;
            }
        }

        state
            .GetCurrentQueue()
            .QueueImmediate(new ALoseArtifact { artifactType = new DroneMkI().Key() });
    }
}