using System.Runtime.CompilerServices;
using CutebaltCore;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Essentials;
using Nickel.ModSettings;
using Shockah.Kokoro;
using Teuria.HotReload;

[assembly: InternalsVisibleTo("GeneratedTextTransformation")]

namespace Teuria.StarcourseDock;

public sealed partial class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<string> CsharpAnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<string> CsharpLocalizations;
    internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }
    internal IKokoroApi KokoroAPI { get; }
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
        Sprites.Register(package, helper);

        Registerables.Register(package, helper);
        Patchables.Patch(Harmony);
        ManualPatchables.Patch(Harmony);
    }
}