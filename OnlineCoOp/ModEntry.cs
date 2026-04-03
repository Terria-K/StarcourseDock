using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Riptide.Utils;
using Teuria.OnlineCoOp.TheClient;
using Teuria.OnlineCoOp.TheServer;

namespace Teuria.OnlineCoOp;

public sealed partial class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;
    internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }

    internal ClientManager ClientManager { get; private set; }
    internal ServerManager ServerManager { get; private set; }

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger)
        : base(package, helper, logger)
    {
        ClientManager = new ClientManager();
        ServerManager = new ServerManager();

        RiptideLogger.Initialize(
            (x) => logger.LogDebug("Riptide: {x}", x),
            (x) => logger.LogInformation("Riptide: {x}", x),
            (x) => logger.LogWarning("Riptide: {x}", x),
            (x) => logger.LogError("Riptide: {x}", x), 
            true
        );

        Instance = this;
        Harmony = helper.Utilities.Harmony;

		this.AnyLocalizations = new YamlLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.yaml").OpenRead()
		);
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);

        helper.Utilities.Harmony.PatchAll(typeof(ModEntry).Assembly);
    }   
}