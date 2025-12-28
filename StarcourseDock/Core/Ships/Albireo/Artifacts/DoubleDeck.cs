using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal class DoubleDeck : Artifact, IRegisterable
{
    public bool isOrange;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact(
            "DoubleDeck",
            new()
            {
                ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
                Meta = new()
                {
                    owner = Deck.colorless,
                    pools = [ArtifactPool.EventOnly],
                    unremovable = true,
                },
                Sprite = Sprites.artifacts_DoubleDeck.Sprite,
                Name = Localization.ship_Albireo_artifact_DoubleDeck_name(),
                Description = Localization.ship_Albireo_artifact_DoubleDeck_description()
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips() => [AlbireoKit.GetPolarityTraitTooltip(), Polarity.GetTooltip()];

    public override void OnCombatStart(State state, Combat combat)
    {
        if (isOrange)
        {
            state.ship.Set(Polarity.PolarityOrangeEntry.Status);
        }
        else
        {
            state.ship.Set(Polarity.PolarityBlueEntry.Status);
        }
    }

    public override void OnPlayerDeckShuffle(State state, Combat combat)
    {
        combat.Queue(new APolaritySwitch());
        Polarity.SwitchPolarity(state);
        isOrange = !isOrange;
    }

    public override Spr GetSprite()
    {
        if (isOrange)
        {
            return Sprites.artifacts_DoubleDeck_Orange.Sprite;
        }

        return Sprites.artifacts_DoubleDeck.Sprite;
    }
}
