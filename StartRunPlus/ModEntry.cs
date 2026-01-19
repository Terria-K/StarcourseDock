using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Teuria.HotReloader;

[assembly: InternalsVisibleTo("GeneratedTextTransformation")]

namespace Teuria.StartRunPlus;

public sealed partial class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;
    internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger)
        : base(package, helper, logger)
    {
        Instance = this;
        Harmony = helper.Utilities.Harmony;

		this.AnyLocalizations = new YamlLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.yaml").OpenRead()
		);
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);

        HotReload.Initialize(
            package,
            logger, 
            helper.Utilities.Harmony
        );

        Registerables.Register(package, helper);
        Registerables.Patch(helper.Utilities.Harmony);
    }   
}

internal interface IRegisterable
{
    abstract static void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}