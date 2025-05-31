using FSPRO;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class ColdStatus
    : IRegisterable,
        IKokoroApi.IV2.IStatusLogicApi.IHook,
        IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry ColdEntry { get; private set; } = null!;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        ColdEntry = helper.Content.Statuses.RegisterStatus(
            "Cold",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["status", "Cold", "name"]).Localize,
                Description = ModEntry
                    .Instance.AnyLocalizations.Bind(["status", "Cold", "description"])
                    .Localize,
                Definition = new()
                {
                    isGood = false,
                    color = new Color("70b2ff"),
                    border = new Color("7fe2ff"),
                    icon = Sprites.cold.Sprite,
                    affectedByTimestop = true,
                },
                ShouldFlash = (State s, Combat c, Ship ship, Status status) =>
                    ship.Get(status) >= 3,
            }
        );

        var coldStatus = new ColdStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusLogic.RegisterHook(coldStatus, 0);
        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(coldStatus, 0);
    }

    public void OnStatusTurnTrigger(
        IKokoroApi.IV2.IStatusLogicApi.IHook.IOnStatusTurnTriggerArgs args
    )
    {
        if (args.Status != ColdEntry.Status)
        {
            return;
        }

        if (args.OldAmount >= 3)
        {
            args.Combat.Queue(new AFreeze() { targetPlayer = args.Ship.isPlayerShip });
            return;
        }

        if (args.Timing == IKokoroApi.IV2.IStatusLogicApi.StatusTurnTriggerTiming.TurnStart)
        {
            args.Ship.Add(ColdEntry.Status, -1);
            Audio.Play(Event.Status_PowerDown);
        }
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args
    )
    {
        if (args.Status != ColdEntry.Status)
        {
            return args.Tooltips;
        }

        return [.. args.Tooltips, .. AFreeze.GetTooltipsGlobal()];
    }
}
