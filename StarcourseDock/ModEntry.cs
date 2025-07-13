using System.Runtime.CompilerServices;
using CutebaltCore;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Essentials;
using Shockah.Kokoro;

[assembly: InternalsVisibleTo("GeneratedTextTransformation")]

namespace Teuria.StarcourseDock;

public sealed partial class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;
    internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }
    internal IKokoroApi KokoroAPI { get; }
    internal IEssentialsApi EssentialAPI { get; }

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger)
        : base(package, helper, logger)
    {
        Instance = this;
        Harmony = helper.Utilities.Harmony;

        this.AnyLocalizations = new YamlLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale =>
                package.PackageRoot.GetRelativeFile($"i18n/{locale}.yaml").OpenRead()
        );
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(
                this.AnyLocalizations
            )
        );

        KokoroAPI = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        EssentialAPI = helper.ModRegistry.GetApi<IEssentialsApi>("Nickel.Essentials")!;
        Sprites.Register(package, helper);

        Registerables.Register(package, helper);
        Patchables.Patch(Harmony);
        ManualPatchables.Patch(Harmony);
    }
}
