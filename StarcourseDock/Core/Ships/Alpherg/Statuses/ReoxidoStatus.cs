using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class ReoxidoStatus : IRegisterable,
    IKokoroApi.IV2.IStatusLogicApi.IHook,
    IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry Reoxido { get; internal set; } = null!;

    private static readonly Lazy<HashSet<Status>> StatusesToCallTurnTriggerHooksFor = new(() => [
		Status.corrode,
		Reoxido.Status,
	]);

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        Reoxido = helper.Content.Statuses.RegisterStatus(
            "Reoxido",
            new()
            {
                Name = Localization.ship_Alpherg_status_Reoxido_name(),
                Description = Localization.ship_Alpherg_status_Reoxido_description(),
                Definition = new()
                {
                    color = new Color("63c74c"),
                    isGood = false,
                    icon = Sprites.icons_reoxido.Sprite,
                    affectedByTimestop = true
                },
                ShouldFlash = (_, _, ship, status) => ship.Get(status) >= 4
            }
        );

        var reoxidoStatus = new ReoxidoStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(reoxidoStatus, 0);
        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(reoxidoStatus, 0);

        ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
		{
			SetShouldCorrode(state.ship, false);
		});
    }

	public IReadOnlySet<Status> GetStatusesToCallTurnTriggerHooksFor(IKokoroApi.IV2.IStatusLogicApi.IHook.IGetStatusesToCallTurnTriggerHooksForArgs args)
		=> StatusesToCallTurnTriggerHooksFor.Value;

	public double ModifyStatusTurnTriggerPriority(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusTurnTriggerPriorityArgs args)
		=> args.Status == Reoxido.Status ? args.Priority + 10 : args.Priority;

    public bool HandleStatusTurnAutoStep(IKokoroApi.IV2.IStatusLogicApi.IHook.IHandleStatusTurnAutoStepArgs args)
    {
        if (args.Timing != IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnEnd)
        {
            return false;
        }

        if (args.Status == Status.corrode && (args.Ship.Get(Reoxido.Status) >= 4 || GetShouldCorrode(args.Ship)))
        {
            args.Amount += 1;
            args.SetStrategy = IKokoroApi.IV2.IStatusLogicApi.StatusTurnAutoStepSetStrategy.Direct;
            SetShouldCorrode(args.Ship, false);
            return false;
        }

        if (args.Status == Reoxido.Status)
        {
            if (args.Amount >= 4)
            {
                args.Amount = 0;
                args.SetStrategy = IKokoroApi.IV2.IStatusLogicApi.StatusTurnAutoStepSetStrategy.QueueSet;
                SetShouldCorrode(args.Ship, true);
            }
            else
            {
                args.Amount -= 1;
                args.SetStrategy = IKokoroApi.IV2.IStatusLogicApi.StatusTurnAutoStepSetStrategy.QueueSet;
            }

            return false;
        }

        return false;
    }

    private static bool GetShouldCorrode(Ship ship)
    {
        return ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(ship, "ShouldCorrode");
    }

    private static void SetShouldCorrode(Ship ship, bool flag)
    {
        ModEntry.Instance.Helper.ModData.SetOptionalModData<bool>(ship, "ShouldCorrode", flag);
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        foreach (var tooltip in args.Tooltips)
        {
            if (tooltip is TTGlossary glossary && glossary.key == $"status.{Reoxido.Status}")
            {
                glossary.vals = [$"<c=boldPink>4</c>"];
            }
        }

        return args.Tooltips;
    }
}
