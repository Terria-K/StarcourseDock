using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class Polarity : IRegisterable
{
    public static IStatusEntry PolarityOrangeEntry { get; internal set; } = null!;
    public static IStatusEntry PolarityBlueEntry { get; internal set; } = null!;


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        PolarityOrangeEntry = helper.Content.Statuses.RegisterStatus(
            "PolarityOrange",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "status", "Polarity", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "status", "Polarity", "orange", "description"]).Localize,
                Definition = new()
                {
                    color = new Color("ff7c19"),
                    isGood = true,
                    icon = Sprites.icons_status_polarity_orange.Sprite,
                    affectedByTimestop = false
                }
            }
        );

        PolarityBlueEntry = helper.Content.Statuses.RegisterStatus(
            "PolarityBlue",
            new()
            {
                Name = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "status", "Polarity", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["ship", "Albireo", "status", "Polarity", "blue", "description"]).Localize,
                Definition = new()
                {
                    color = new Color("2f94ff"),
                    isGood = true,
                    icon = Sprites.icons_status_polarity_blue.Sprite,
                    affectedByTimestop = false
                }
            }
        );
    }

    public static void SwitchPolarity(State state)
    {
        if (IsOrangePolarity(state))
        {
            state.ship.Set(PolarityOrangeEntry.Status, 0);
            state.ship.Set(PolarityBlueEntry.Status, 1);
            return;
        }

            state.ship.Set(PolarityOrangeEntry.Status, 1);
            state.ship.Set(PolarityBlueEntry.Status, 0);
    }

    public static bool IsOrangePolarity(State s)
    {
        var status = s.ship.Get(PolarityOrangeEntry.Status);
        if (status == 1)
        {
            return true;
        }

        return false;
    }
}