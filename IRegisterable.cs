using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal interface IRegisterable
{
    abstract static void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}
