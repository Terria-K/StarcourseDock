using System.Reflection;
using CutebaltCore;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed class SiriusMissileBay : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "SiriusMissileBay",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_SiriusMissileBay.Sprite,
                Name = Localization.ship_Sirius_artifact_SiriusMissileBay_name(),
                Description = Localization.ship_Sirius_artifact_SiriusMissileBay_description(),
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (!combat.HasCardOnHand<ToggleMissileBay>())
        {
            combat.Queue(
                new AAddCard { card = new ToggleMissileBay(), destination = CardDestination.Hand }
            );
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return [new TTCard() { card = new ToggleMissileBay() }];
    }
}
