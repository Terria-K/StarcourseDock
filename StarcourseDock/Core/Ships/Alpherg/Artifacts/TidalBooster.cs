using System.Reflection;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class TidalBooster : Artifact, IRegisterable
{
    public bool isRoutedCannonNotActive;
    public int infradriveCap;
    public bool isPlayerAttack;
    public const int InfradriveLimit = 1;

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
        infradriveCap = 0;
        isRoutedCannonNotActive = false;
    }

    public override Spr GetSprite()
    {
        if (isRoutedCannonNotActive)
        {
            return Sprites.artifacts_TidalBooster_BothInactive.Sprite;
        }

        if (infradriveCap < InfradriveLimit)
        {
            return Sprites.artifacts_TidalBooster.Sprite;
        }

        return Sprites.artifacts_TidalBooster_BlueInactive.Sprite;
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        var routedCannon = state.GetArtifactFromColorless<RoutedCannon>();

        if (routedCannon is not null)
        {
            isRoutedCannonNotActive = routedCannon.disabled;
        }
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        isPlayerAttack = true;
    }

    public override void OnEnemyGetHit(State state, Combat combat, Part? part)
    {
        if (isRoutedCannonNotActive)
        {
            return;
        }

        if (part is null)
        {
            return;
        }

        if (!isPlayerAttack)
        {
            return;
        }

        isPlayerAttack = true;

        var piscium = state.GetArtifactFromColorless<Piscium>();
        if (piscium is null)
        {
            return;
        }

        if (piscium.isRight)
        {
            combat.otherShip.Add(ModEntry.Instance.KokoroAPI.V2.OxidationStatus.Status, 1);
            Pulse();
        }
        else if (infradriveCap < InfradriveLimit)
        {
            combat.otherShip.Add(InfradriveStatus.Infradrive.Status, 1);
            infradriveCap += 1;
            Pulse();
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [
            InfradriveStatus.GetTooltip(1),
            ..StatusMeta.GetTooltips(
                ModEntry.Instance.KokoroAPI.V2.OxidationStatus.Status, 
                ModEntry.Instance.KokoroAPI.V2.OxidationStatus.GetOxidationStatusThreshold(MG.inst.g.state, MG.inst.g.state.ship))
        ];
    }
}
