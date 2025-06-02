using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}
