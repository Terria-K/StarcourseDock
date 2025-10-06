using System.Collections.Frozen;
using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;

namespace Teuria.StarcourseDock;

internal sealed class GraviShip : IRegisterable 
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        //helper.Content.Ships.RegisterShip(
        //    "Gravi",
        //    new() 
        //    {
        //        Name = Localization.ship_Gravi_name(),
        //        Description = Localization.ship_Gravi_description(),
        //        ExclusiveArtifactTypes = new HashSet<Type>(),
        //        Ship = new() 
        //        {
        //
        //
        //        }
        //    }
        //);
    }
}
