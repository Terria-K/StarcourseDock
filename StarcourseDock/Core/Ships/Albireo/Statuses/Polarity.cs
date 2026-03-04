using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class Polarity : IRegisterable, 
    IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry PolarityStatus { get; internal set; } = null!;


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        PolarityStatus = helper.Content.Statuses.RegisterStatus(
            "Polarity",
            new()
            {
                Name = Localization.ship_Albireo_status_Polarity_name(),
                Description = Localization.ship_Albireo_status_Polarity_orange_description(),
                Definition = new()
                {
                    color = new Color("2f94ff"),
                    isGood = true,
                    icon = Sprites.icons_status_polarity_blue.Sprite,
                    affectedByTimestop = false
                }
            }
        );

        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(new Polarity());
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

    public static bool IsOrangePolarity(State s, out bool isMiddle)
    {
        var deck = s.GetArtifactFromColorless<DoubleDeck>();
        if (deck is null)
        {
            isMiddle = false;
            return false;
        }

        isMiddle = false;
        var status = s.ship.Get(PolarityStatus.Status);
        if (status > deck.HalfMax)
        {
            return true;
        }

        if (status == deck.HalfMax)
        {
            isMiddle = true;
            return true;
        }

        return false;
    }

    public (IReadOnlyList<Color> Colors, int? BarSegmentWidth)? OverrideStatusRenderingAsBars(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusRenderingAsBarsArgs args)
    {
        if (args.Status != PolarityStatus.Status)
        {
            return null;
        }

        int cap = args.State.HasArtifactFromColorless<DoublePolarity>() || args.State.HasArtifactFromColorless<DoubleCard>() ? 4 : 2;
        int half = cap / 2;

        var colors = new Color[cap + 1];

        int amount = args.State.ship.Get(PolarityStatus.Status) - 1;

        for (int i = 0; i < colors.Length; i += 1)
        {
            if (i < half)
            {
                if (amount <= i)
                {
                    colors[i] = new Color("2f94ff");
                }
                else
                {
                    colors[i] = new Color("004896");
                }
            }

            if (i == half)
            {
                colors[i] = Colors.white;
            }

            if (i > half)
            {
                if (amount >= i)
                {
                    colors[i] = new Color("ff7c19");
                }
                else
                {
                    colors[i] = new Color("cc000e");
                }
            }
        }

        return (colors, null);
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args)
    {
        if (args.Status == PolarityStatus.Status)
        {
            var deck = MG.inst.g.state.GetArtifactFromColorless<DoubleDeck>();
            if (deck is null)
            {
                return args.Tooltips;
            }

            if (args.Ship?.Get(args.Status) > deck.HalfMax)
            {
                return [
                    new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PolarityStatus")
                    {
                        Title = Localization.Str_ship_Albireo_status_Polarity_name(),
                        Description = Localization.Str_ship_Albireo_status_Polarity_orange_description(),
                        Icon = Sprites.icons_status_polarity.Sprite,
                        TitleColor = Colors.status
                    }
                ];
            }

            if (args.Ship?.Get(args.Status) < deck.HalfMax)
            {
                return [
                    new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PolarityStatus")
                    {
                        Title = Localization.Str_ship_Albireo_status_Polarity_name(),
                        Description = Localization.Str_ship_Albireo_status_Polarity_blue_description(),
                        Icon = Sprites.icons_status_polarity.Sprite,
                        TitleColor = Colors.status
                    }
                ];
            }

            if (args.Ship?.Get(args.Status) == deck.HalfMax)
            {
                return [
                    new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::PolarityStatus")
                    {
                        Title = Localization.Str_ship_Albireo_status_Polarity_name(),
                        Description = Localization.Str_ship_Albireo_status_Polarity_white_description(),
                        Icon = Sprites.icons_status_polarity.Sprite,
                        TitleColor = Colors.status
                    }
                ];
            }
        }
        return args.Tooltips;
    }
}