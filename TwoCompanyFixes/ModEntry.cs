using Nickel;
using Nanoray.PluginManager;
using Nanoray.PluginManager.Cecil;
using Microsoft.Extensions.Logging;

namespace Teuria.TwoCompanyFixes;

public sealed class ModEntry : SimpleMod
{
    public static ModEntry Instance { get; private set; } = null!;

    public ModEntry(
        IPluginPackage<IModManifest> package, 
        IModHelper helper, 
        ILogger logger,
        ExtendableAssemblyDefinitionEditor editor) : base(package, helper, logger)
    {
        Instance = this;
        editor.RegisterDefinitionEditor(new FixTwoCompanyMissile());
    }
}
