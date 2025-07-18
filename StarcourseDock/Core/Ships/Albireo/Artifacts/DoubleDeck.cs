using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using CutebaltCore;
using Shockah.Kokoro;

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
                Name = Localization.ship_Albireo_artifact_DoubleDeck_name(),
                Description = Localization.ship_Albireo_artifact_DoubleDeck_description()
            }
        );

        ModEntry.Instance.KokoroAPI.V2.TemporaryUpgrades.RegisterHook(new DoubleDeckTempUpgradeHook());
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

    public override int ModifyCardRewardCount(State state, bool isEvent, bool inCombat)
    {
        if (isEvent || inCombat)
        {
            return 0;
        }

        return -1;
    }
}

file sealed class DoubleDeckTempUpgradeHook : IKokoroApi.IV2.ITemporaryUpgradesApi.IHook
{
    public void OnTemporaryUpgrade(IKokoroApi.IV2.ITemporaryUpgradesApi.IHook.IOnTemporaryUpgradeArgs args)
    {
        if (!ModEntry.Instance.Helper.ModData.TryGetModData(args.Card, "polarity.card.linked", out Card? linkCard) || linkCard is null)
        {
            return;
        }

        if (args.NewTemporaryUpgrade != null)
        {
            ModEntry.Instance.KokoroAPI.V2.TemporaryUpgrades.SetTemporaryUpgrade(args.State, linkCard, args.NewTemporaryUpgrade);
            return;
        }

        // does it actually permanently upgrade if NewTemporaryUpgrade is null?
        ModEntry.Instance.KokoroAPI.V2.TemporaryUpgrades.SetTemporaryUpgrade(args.State, linkCard, args.NewUpgrade);
    }
}
