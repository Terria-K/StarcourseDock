using CutebaltCore;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Essentials;
using Nickel.ModSettings;
using Shockah.Kokoro;
using TheJazMaster.CombatQoL;

namespace Teuria.StarcourseDock;

public sealed partial class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<string> CsharpAnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<string> CsharpLocalizations;
    internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }
    internal IKokoroApi KokoroAPI { get; }
    internal ICombatQolApi? CombatQolAPI { get; }
    internal IModSettingsApi ModSettingsAPI { get; }
    internal IEssentialsApi EssentialAPI { get; }

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger)
        : base(package, helper, logger)
    {
        Instance = this;
        Harmony = helper.Utilities.Harmony;

        CsharpAnyLocalizations = new CsharpLocalizationProvider();
        CsharpLocalizations = new MissingPlaceholderLocalizationProvider<string>(
            new CurrentLocaleOrEnglishLocalizationProvider<string>(
                CsharpAnyLocalizations
            )
        );

        KokoroAPI = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        EssentialAPI = helper.ModRegistry.GetApi<IEssentialsApi>("Nickel.Essentials")!;
        ModSettingsAPI = helper.ModRegistry.GetApi<IModSettingsApi>("Nickel.ModSettings")!;
        CombatQolAPI = helper.ModRegistry.GetApi<ICombatQolApi>("TheJazMaster.CombatQoL");
        Sprites.Register(package, helper);

        Registerables.Register(package, helper);
        Harmony.PatchAll(typeof(ModEntry).Assembly);
    }
}