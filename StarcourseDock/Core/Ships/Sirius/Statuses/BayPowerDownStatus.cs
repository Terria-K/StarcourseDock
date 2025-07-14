using CutebaltCore;
using FSPRO;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class BayPowerDownStatus
    : IRegisterable,
        IKokoroApi.IV2.IStatusLogicApi.IHook,
        IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry BayPowerDownEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        BayPowerDownEntry = helper.Content.Statuses.RegisterStatus(
            "BayPowerDown",
            new()
            {
                Name = Localization.ship_Sirius_status_BayPowerDown_name(),
                Description = Localization.ship_Sirius_status_BayPowerDown_description(),
                Definition = new()
                {
                    isGood = false,
                    color = new Color("ff6f6f"),
                    border = new Color("ff6f6f"),
                    icon = Sprites.icons_power_down.Sprite,
                    affectedByTimestop = true,
                },
            }
        );

        var bayPowerDownStatus = new BayPowerDownStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(bayPowerDownStatus, 0);
        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(bayPowerDownStatus, 0);
    }

    public void OnStatusTurnTrigger(
        IKokoroApi.IV2.IStatusLogicApi.IHook.IOnStatusTurnTriggerArgs args
    )
    {
        if (args.Status != BayPowerDownEntry.Status)
        {
            return;
        }

        if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
        {
            args.Ship.Add(BayPowerDownEntry.Status, -1);
        }
    }

    public static Tooltip PowerDownTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::powerDown")
        {
            Title = Localization.Str_ship_Sirius_icon_powerDown_name(),
            TitleColor = Colors.midrow,
            Description = Localization.Str_ship_Sirius_icon_powerDown_description(),
            Icon = Sprites.icons_power_down.Sprite,
        };
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args
    )
    {
        if (args.Status != BayPowerDownEntry.Status)
        {
            return args.Tooltips;
        }

        return [.. args.Tooltips, PowerDownTooltip()];
    }
}
