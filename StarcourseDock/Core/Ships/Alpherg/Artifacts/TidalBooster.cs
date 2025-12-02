using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class TidalBooster : Artifact, IRegisterable
{
    public int underdriveCap;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "TidalBooster",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.Boss],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_TidalBooster.Sprite,
                Name = Localization.ship_Alpherg_artifact_TidalBooster_name(),
                Description = Localization.ship_Alpherg_artifact_TidalBooster_description(),
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        underdriveCap = 0;
    }

    public override Spr GetSprite()
    {
        if (underdriveCap < 2)
        {
            return Sprites.artifacts_TidalBooster.Sprite;
        }

        return Sprites.artifacts_TidalBooster_BlueInactive.Sprite;
    }

    public override void OnEnemyGetHit(State state, Combat combat, Part? part)
    {
        if (part is null)
        {
            return;
        }
        var piscium = state.GetArtifactFromColorless<Piscium>();
        if (piscium is null)
        {
            return;
        }

        if (piscium.isRight)
        {
            combat.otherShip.Add(Status.heat, 1);
        }
        else if (underdriveCap < 2)
        {
            combat.otherShip.Add(InfradriveStatus.Infradrive.Status, 1);
            underdriveCap += 1;
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            ..StatusMeta.GetTooltips(Status.heat, 3), InfradriveStatus.GetTooltip(1)
        ];
    }
}
