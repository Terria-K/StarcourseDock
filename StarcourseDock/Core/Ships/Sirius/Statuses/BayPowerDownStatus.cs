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
                Name = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "status", "BayPowerDown", "name"]
                    )
                    .Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(
                        ["ship", "Sirius", "status", "BayPowerDown", "description"]
                    )
                    .Localize,
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
            Title = ModEntry.Instance.Localizations.Localize(
                ["ship", "Sirius", "icon", "powerDown", "name"]
            ),
            TitleColor = Colors.midrow,
            Description = ModEntry.Instance.Localizations.Localize(
                ["ship", "Sirius", "icon", "powerDown", "description"]
            ),
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
