using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.HotReloader;

public static class HotReload
{
    internal static ILogger? logger = null!;

    /// <summary>
    /// Initialize the HotReloadWatcher instance, this should also initialize the extension as well.
    /// </summary>
    /// <param name="projectPath">The mod's project path. Can relative to Nickel root, or absolute to your filesystem</param>
    /// <param name="package">The mod's package.</param>
    /// <param name="logger">The mod's logger.</param>
    /// <param name="harmony">The Nickel.IModUtilities.Harmony instance for this mod, used for patching methods.</param>
    /// <returns>An <see cref="IHotReloadWatcher"/> interface for a utility to hot reload assets</returns>
    public static IHotReloadWatcher Initialize(
        IPluginPackage<IModManifest> package, 
        ILogger? logger,
        IHarmony harmony)
    {
#if DEBUG
        HotReload.logger = logger;
        return new HotReloadWatcher(package, harmony);
#else
        return new RevokeHotReload();
#endif
    }
}

/// <summary>
/// An interface for a utility to hot reload assets.
/// </summary>
public interface IHotReloadWatcher
{
    /// <summary>
    /// Indicates if the watcher is reloading.
    /// </summary>
    public bool IsReloading { get; }
    /// <summary>
    /// A file system watcher used for reloading.
    /// </summary>
    public FileSystemWatcher? Watcher { get; }

    /// <summary>
    /// Add a delegate that calls everytime the watcher reloads.
    /// </summary>
    /// <param name="action">A delegate to be called when the watcher reloads</param>
    public void Add(Action action);
}

internal sealed class RevokeHotReload : IHotReloadWatcher
{
    public FileSystemWatcher? Watcher => null;
    public bool IsReloading => false;

    public void Add(Action action) {}
}

/// <summary>
/// A utility to hot reload assets.
/// </summary>
internal sealed class HotReloadWatcher : IHotReloadWatcher
{
    private bool reload;
    private readonly List<Action> funcReload = [];
    private readonly object watchLock = new object();
    public static HotReloadWatcher Instance { get; private set; } = null!;
    public FileSystemWatcher? Watcher { get; }

    public bool IsReloading => reload;

    internal HotReloadWatcher(IPluginPackage<IModManifest> package, IHarmony harmony) 
    {
        harmony.Patch(
            AccessTools.DeclaredMethod(typeof(State), nameof(State.Update)),
            postfix: new HarmonyMethod(HotReloadUpdateAssets)
        );
        

        Instance = this;

        Watcher = new FileSystemWatcher
        {
            Path = package.PackageRoot.FullName,
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
        };

        Watcher.Changed += HotReloadUpdate;
    }

    public void Add(Action action)
    {
        funcReload.Add(action);
    }

    private void HotReloadUpdate(object sender, FileSystemEventArgs e)
    {
        lock (watchLock)
        {
            Instance.reload = true;
        }
    }

    private static void HotReloadUpdateAssets()
    {
        lock (Instance.watchLock)
        {
            if (Instance.reload)
            {
                foreach (var reload in Instance.funcReload)
                {
                    reload();
                }

                Instance.reload = false;
            }
        }
    }
}

/// <summary>
/// An extension for providing reloadable contents.
/// </summary>
public static class SpriteExtensions
{
    /// <summary>
    /// Registers a reloadable sprite, with image data coming from a file.
    /// The file's path will be used for the content name.
    /// </summary>
    /// <param name="sprites">A mod-specific sprite registry. Allows looking up and registering sprites</param>
    /// <param name="fileInfo">A file to load an image data from</param>
    /// <returns>A new sprite entry</returns>
    public static ISpriteEntry RegisterReloadableSprite(this IModSprites sprites, IFileInfo fileInfo)
    {
#if DEBUG
        if (HotReloadWatcher.Instance is null)
        {
            return sprites.RegisterSprite(fileInfo);
        }

        var holder = sprites.RegisterSprite(fileInfo);
        
        var sprite = sprites.RegisterDynamicSprite(() => 
        {
            return SpriteLoader.Get(holder.Sprite)!;
        });
        
        HotReloadWatcher.Instance.Add(() => {
            holder = sprites.RegisterSprite(
                fileInfo
            );

            HotReload.logger?.LogInformation("Successfully hot reloaded path: {path}", fileInfo.Name);
        });

        return sprite;
#else
        return sprites.RegisterSprite(fileInfo);
#endif
    }

    /// <summary>
    /// Registers a reloadable sprite, with image data coming from a file.
    /// </summary>
    /// <param name="sprites">A mod-specific sprite registry. Allows looking up and registering sprites.</param>
    /// <param name="name">The name for the content.</param>
    /// <param name="fileInfo">A file to load an image data from.</param>
    /// <returns>A new sprite entry</returns>
    public static ISpriteEntry RegisterReloadableSprite(this IModSprites sprites, string name, IFileInfo fileInfo)
    {
#if DEBUG
        if (HotReloadWatcher.Instance is null)
        {
            return sprites.RegisterSprite(name, fileInfo);
        }

        var holder = sprites.RegisterSprite(fileInfo);
        
        var sprite = sprites.RegisterDynamicSprite(() => 
        {
            return SpriteLoader.Get(holder.Sprite)!;
        });
        
        HotReloadWatcher.Instance.Add(() => {
            holder = sprites.RegisterSprite(
                fileInfo
            );

            HotReload.logger?.LogInformation("Successfully hot reloaded name: {name} with path: {path}", name, fileInfo.Name);
        });

        return sprite;
#else
        return sprites.RegisterSprite(name, fileInfo);
#endif
    }
}