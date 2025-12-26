using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class SeriousDedication : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SeriousDedication",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                },
                Sprite = Sprites.artifacts_SeriousDedication.Sprite,
                Name = Localization.ship_WolfRayet_artifact_SeriousDedication_name(),
                Description = Localization.ship_WolfRayet_artifact_SeriousDedication_description(),
            }
        );
    }

    public override void OnReceiveArtifact(State state)
    {
        for (int i = 0; i < state.ship.parts.Count; i++)
        {
            var part = state.ship.parts[i];
            if (part.key == "rayet_missiles")
            {
                part.type = WolfRayetShip.MissilePartType.PartType;
                part.stunModifier = WolfRayetShip.HotStunModifier.PartStunModifier;
                part.skin = WolfRayetShip.MissileLeftSlot.UniqueName;
                part.active = false;
                continue;
            }

            if (part.key == "rayet_cannon")
            {
                part.type = WolfRayetShip.MissilePartType.PartType;
                part.stunModifier = WolfRayetShip.HotStunModifier.PartStunModifier;
                part.skin = WolfRayetShip.MissileRightSlot.UniqueName;
                part.active = false;
            }
        }
    }
}
