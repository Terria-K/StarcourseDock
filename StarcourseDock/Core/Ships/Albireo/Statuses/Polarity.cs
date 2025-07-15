using System.Runtime.InteropServices;
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
                Name = Localization.ship_Albireo_status_Polarity_name(),
                Description = Localization.ship_Albireo_status_Polarity_orange_description(),
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
                Name = Localization.ship_Albireo_status_Polarity_name(),
                Description = Localization.ship_Albireo_status_Polarity_blue_description(),
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

    public static Tooltip GetTooltip()
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PolarityStatus")
        {
            Title = Localization.Str_ship_Albireo_status_Polarity_name(),
            Description = Localization.Str_ship_Albireo_status_Polarity_description(),
            Icon = Sprites.icons_status_polarity.Sprite,
            TitleColor = Colors.status
        };
    }

    public static void SwitchPolarity(State state)
    {
        var parts = CollectionsMarshal.AsSpan(state.ship.parts);

        if (IsOrangePolarity(state))
        {
            state.ship.Set(PolarityOrangeEntry.Status, 0);
            state.ship.Set(PolarityBlueEntry.Status, 1);

            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                switch (p.key)
                {
                    case "ab_missiles":
                        p.skin = AlbireoShip.AlbireoMissileBayBlue.UniqueName;
                        break;
                    case "ab_cannon":
                        p.skin = AlbireoShip.AlbireoCannonBlue.UniqueName;
                        break;
                    case "ab_cockpit":
                        p.skin = AlbireoShip.AlbireoCockpitBlue.UniqueName;
                        break;
                }
            }
            return;
        }

        state.ship.Set(PolarityOrangeEntry.Status, 1);
        state.ship.Set(PolarityBlueEntry.Status, 0);

        for (int i = 0; i < parts.Length; i++)
        {
            var p = parts[i];
            switch (p.key)
            {
                case "ab_missiles":
                    p.skin = AlbireoShip.AlbireoMissileBayOrange.UniqueName;
                    break;
                case "ab_cannon":
                    p.skin = AlbireoShip.AlbireoCannonOrange.UniqueName;
                    break;
                case "ab_cockpit":
                    p.skin = AlbireoShip.AlbireoCockpitOrange.UniqueName;
                    break;
            }
        }
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