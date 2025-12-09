using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

namespace Teuria.StarcourseDock;

internal sealed class WrathChargeStatus : IRegisterable,
    IKokoroApi.IV2.IStatusRenderingApi.IHook
{
    public static IStatusEntry WrathCharge { get; internal set; } = null!;


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        WrathCharge = helper.Content.Statuses.RegisterStatus(
            "WrathCharge",
            new()
            {
                Name = Localization.ship_Charon_status_WrathCharge_name(),
                Description = Localization.ship_Charon_status_WrathCharge_description("1", "2", "1"),
                Definition = new()
                {
                    color = new Color("8f1224"),
                    isGood = true,
                    icon = Sprites.icons_wrathCharge.Sprite,
                },
                ShouldFlash = (s, c, ship, status) =>
                {
                    var hasSanity = s.HasArtifactFromColorless<SanityExpansion>();
                    int requiredSanity = hasSanity ? 2 : 1;
                    
                    return ship.Get(status) > requiredSanity;
                }
            }
        );

        var wrathChargeStatus = new WrathChargeStatus();

        ModEntry.Instance.KokoroAPI.V2.StatusRendering.RegisterHook(wrathChargeStatus, 0);
    }

    public static Tooltip GetTooltip(int damage, int times, int takeDamage)
    {
        return new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::WrathChargeStatus")
        {
            Title = Localization.Str_ship_Charon_status_WrathCharge_name(),
            Description = Localization.Str_ship_Charon_status_WrathCharge_description(damage.ToString(), times.ToString(), takeDamage.ToString()),
            Icon = Sprites.icons_wrathCharge.Sprite,
            TitleColor = Colors.status
        };
    }

    public IReadOnlyList<Tooltip> OverrideStatusTooltips(
        IKokoroApi.IV2.IStatusRenderingApi.IHook.IOverrideStatusTooltipsArgs args
    )
    {
        if (args.Status != WrathCharge.Status)
        {
            return args.Tooltips;
        }

        var state = MG.inst.g.state;

        int times = 2;
        if (state.HasArtifactFromColorless<SanityExpansion>())
        {
            times += 1;
        }

        int damage = 1;
        if (state.HasArtifactFromColorless<EnrageDrill>())
        {
            damage += 1;
        }

        return [GetTooltip(Card.GetActualDamage(state, 1), times, damage)];
    }
}