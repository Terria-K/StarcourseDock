using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StartRunPlus;

internal static class Registerables
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        StartRunPlus.Register(package, helper);
    }

    public static void Patch(IHarmony harmony)
    {
        StartRunPlus.Patch(harmony);
    }
}