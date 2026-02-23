using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class RoutedCannon : Artifact, IRegisterable
{
    public bool disabled;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "RoutedCannon",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_RoutedCannon.Sprite,
                Name = Localization.ship_Alpherg_artifact_RoutedCannon_name(),
                Description = Localization.ship_Alpherg_artifact_RoutedCannon_description(),
            }
        );
    }

    public override Spr GetSprite()
    {
        if (disabled)
        {
            return Sprites.artifacts_RoutedCannonInactive.Sprite;
        }
        return Sprites.artifacts_RoutedCannon.Sprite;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (disabled)
        {
            disabled = false;
        }

        combat.Queue(new AModifyCannon() { active = false });
    }

    public override void OnPlayerPlayCard(
        int energyCost,
        Deck deck,
        Card card,
        State state,
        Combat combat,
        int handPosition,
        int handCount
    )
    {
        if (card is RerouteCannon rerouteCannon)
        {
            if (rerouteCannon.upgrade == Upgrade.B)
            {
                return;
            }
            disabled = true;
        }
    }
}
