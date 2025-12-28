using System.Runtime.CompilerServices;
using CutebaltCore;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Essentials;
using Nickel.ModSettings;
using Shockah.Kokoro;

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


        //HotReloadExperiment();
        KokoroAPI = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        EssentialAPI = helper.ModRegistry.GetApi<IEssentialsApi>("Nickel.Essentials")!;
        ModSettingsAPI = helper.ModRegistry.GetApi<IModSettingsApi>("Nickel.ModSettings")!;
        Sprites.Register(package, helper);

        Registerables.Register(package, helper);
        Patchables.Patch(Harmony);
        ManualPatchables.Patch(Harmony);
    }

    //public static ISpriteEntry TestSprite { get; private set; } =  null!;
    //
    //private void HotReloadExperiment()
    //{
    //    var watcher = new HotReloadWatcher(Package, Harmony);
    //    var holder = Helper.Content.Sprites.RegisterSprite(
    //        Package.PackageRoot.GetRelativeFile("assets/parts/albireo_chassis.png")
    //    );
    //
    //    TestSprite = Helper.Content.Sprites.RegisterDynamicSprite(() => 
    //    {
    //        return SpriteLoader.Get(holder.Sprite)!;
    //    });
    //
    //    watcher.AddToReload(() => {
    //        holder = Helper.Content.Sprites.RegisterSprite(
    //            Package.PackageRoot.GetRelativeFile("assets/parts/albireo_chassis.png")
    //        );
    //    });
    //}
}

//internal sealed class HotReloadWatcher 
//{
//    private bool reload;
//    private readonly List<Action> funcReload = [];
//    private readonly object watchLock = new object();
//    public static HotReloadWatcher Instance { get; private set; } = null!;
//    public FileSystemWatcher Watcher { get; }
//
//    public HotReloadWatcher(IPluginPackage<IModManifest> package, IHarmony harmony) 
//    {
//        harmony.Patch(
//            AccessTools.DeclaredMethod(typeof(State), nameof(State.Update)),
//            postfix: new HarmonyMethod(HotReloadUpdateAssets)
//        );
//
//        Instance = this;
//        Console.WriteLine(package.PackageRoot.AsDirectory!.FullName);
//        Watcher = new FileSystemWatcher(package.PackageRoot.AsDirectory!.FullName);
//
//        Watcher.Changed += HotReloadUpdate;
//        Watcher.EnableRaisingEvents = true;
//        Watcher.IncludeSubdirectories = true;
//    }
//
//    public void AddToReload(Action action)
//    {
//        funcReload.Add(action);
//    }
//
//    private void HotReloadUpdate(object sender, FileSystemEventArgs e)
//    {
//        lock (watchLock)
//        {
//            Instance.reload = true;
//        }
//    }
//
//    private static void HotReloadUpdateAssets()
//    {
//        lock (Instance.watchLock)
//        {
//            if (Instance.reload)
//            {
//                foreach (var reload in Instance.funcReload)
//                {
//                    reload();
//                }
//
//                Instance.reload = false;
//            }
//        }
//    }
//}
