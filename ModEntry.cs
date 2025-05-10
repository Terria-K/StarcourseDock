using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Kokoro;

[assembly: InternalsVisibleTo("GeneratedTextTransformation")]

namespace Teuria.StarcourseDock;

public sealed class ModEntry : SimpleMod
{
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;
	internal static ModEntry Instance { get; private set; } = null!;
    internal IHarmony Harmony { get; }
    internal IKokoroApi KokoroAPI { get; }

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = helper.Utilities.Harmony;

		this.AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
		);
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);

        KokoroAPI = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        Sprites.Register(package, helper);
        Registerables.Register(package, helper);

        // AccessTools.DeclaredMethod(typeof(SpicaShip), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(ShieldOrShot), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(FixedStar), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(Shrink), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(DodgeOrShift), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

        // AccessTools.DeclaredMethod(typeof(AlphergShip), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(Piscium), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(RoutedCannon), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(RerouteCannon), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

        // AccessTools.DeclaredMethod(typeof(AlbireoShip), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(DoubleStar), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        // AccessTools.DeclaredMethod(typeof(RelativeMotion), nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
    }
}
