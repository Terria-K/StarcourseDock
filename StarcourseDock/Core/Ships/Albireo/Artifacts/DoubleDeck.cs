using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace Teuria.StarcourseDock;

internal class DoubleDeck : Artifact, IRegisterable
{
    public bool isOrange;
    public int count = 2;
    public int maxCapacity = 3;

    [JsonIgnore]
    public int HalfMax => (int)Math.Ceiling(maxCapacity / 2.0);

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

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn == 1)
        {
            state.ship.Set(Polarity.PolarityStatus.Status, count);
            if (count < HalfMax && isOrange)
            {
                combat.QueueImmediate(new APolaritySwitch());
                isOrange = false;
            }

            if (count > HalfMax && !isOrange)
            {
                combat.QueueImmediate(new APolaritySwitch());
                isOrange = true;
            }
        }
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        if (handCount <= 1)
        {
            return;
        }

        if ((handCount == 3 && (handPosition == 0 || handPosition == 2)) ||
            (handCount != 3) && (handPosition <= 1 || handPosition >= handCount - 2))
        {
            int delta = -Math.Sign(handCount - 1 - 2 * handPosition);
            count = MathHelper.Clamp(count + delta, 1, maxCapacity);
            state.ship.Set(Polarity.PolarityStatus.Status, count);
        }

        if (count < HalfMax && isOrange)
        {
            combat.QueueImmediate(new APolaritySwitch() { isOrange = !isOrange });
            isOrange = false;
        }

        if (count > HalfMax && !isOrange)
        {
            combat.QueueImmediate(new APolaritySwitch() { isOrange = !isOrange });
            isOrange = true;
        }
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
