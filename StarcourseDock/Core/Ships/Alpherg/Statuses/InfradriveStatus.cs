using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class InfradriveStatus : IRegisterable,
    IKokoroApi.IV2.IStatusLogicApi.IHook,
    IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry Infradrive { get; internal set; } = null!;


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Infradrive = helper.Content.Statuses.RegisterStatus(
            "Infradrive",
            new()
            {
                Name = Localization.ship_Alpherg_status_Infradrive_name(),
                Description = Localization.ship_Alpherg_status_Infradrive_description("1"),
                Definition = new()
                {
                    color = new Color("3a7d47"),
                    isGood = false,
                    icon = Sprites.icons_infradrive.Sprite
                },
            }
        );

        var infradriveStatus = new InfradriveStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(infradriveStatus, 0);
        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(infradriveStatus, 0);
    }

    public static Tooltip GetTooltip(int amount)
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::InfradriveStatus")
        {
            Title = Localization.Str_ship_Alpherg_status_Infradrive_name(),
            Description = Localization.Str_ship_Alpherg_status_Infradrive_description(amount.ToString()),
            Icon = Sprites.icons_infradrive.Sprite,
            TitleColor = Colors.status
        };
    }

    public void OnStatusTurnTrigger(
        IKokoroApi.IV2.IStatusLogicApi.IHook.IOnStatusTurnTriggerArgs args
    )
    {
        if (args.Status != Infradrive.Status)
        {
            return;
        }

        if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd)
        {
            args.Ship.Set(Infradrive.Status, 0);
        }
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args
    )
    {
        if (args.Status != Infradrive.Status)
        {
            return args.Tooltips;
        }

        return [ GetTooltip(args.Amount)];
    }
}