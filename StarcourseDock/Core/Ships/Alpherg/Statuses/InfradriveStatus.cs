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
                Description = Localization.ship_Alpherg_status_Infradrive_description(),
                Definition = new()
                {
                    color = new Color("3a7d47"),
                    isGood = false,
                    icon = Sprites.icons_infradrive.Sprite,
                    affectedByTimestop = true
                },
            }
        );

        var infradriveStatus = new InfradriveStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(infradriveStatus, 0);
        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(infradriveStatus, 0);
    }

	public double ModifyStatusTurnTriggerPriority(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusTurnTriggerPriorityArgs args)
		=> args.Status == Infradrive.Status ? args.Priority + 10 : args.Priority;

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd)
        {
            return false;
        }

        if (args.Status != Infradrive.Status)
        {
            return false;
        }

        args.Amount = 0;

        return false;
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        foreach (var tooltip in args.Tooltips)
        {
            if (tooltip is TTGlossary glossary && glossary.key == $"status.{Infradrive.Status}")
            {
                glossary.vals = [$"<c=boldPink>{args.Amount}</c>"];
            }
        }

        return args.Tooltips;
    }
}