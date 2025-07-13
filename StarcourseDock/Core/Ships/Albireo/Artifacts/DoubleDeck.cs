using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed class DoubleDeck : Artifact, IRegisterable
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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Albireo", "artifact", "DoubleDeck", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Albireo", "artifact", "DoubleDeck", "description"]
                    )
                    .Localize,
            }
        );
    }

    public override List<Tooltip>? GetExtraTooltips() => [AlbireoKit.GetPolarityTraitTooltip()];

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

    public override int ModifyCardRewardCount(State state, bool isEvent, bool inCombat)
    {
        if (isEvent || inCombat)
        {
            return 0;
        }

        return -1;
    }
}
