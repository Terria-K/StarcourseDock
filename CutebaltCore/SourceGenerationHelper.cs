namespace CutebaltCore;

internal static class SourceGenerationHelper
{
    public const string IRegisterable = """
        using Nanoray.PluginManager;
        using Nickel;

        namespace CutebaltCore;

        internal interface IRegisterable
        {
            static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
        }
        """;

    public const string IPatchable = """
        using Nickel;

        namespace CutebaltCore;

        internal interface IPatchable
        {
            static abstract void Patch(IHarmony harmony);
        }
        """;

    public const string IReversePatchable = """
        using Nickel;

        namespace CutebaltCore;

        internal interface IManualPatchable
        {
            static abstract void ManualPatch(IHarmony harmony);
        }
        """;

    public const string Attributes = """
        #pragma warning disable CS9113
        using Nickel;

        namespace CutebaltCore;

        internal sealed class OnPrefix<T>(string methodName) : Attribute;
        internal sealed class OnPostfix<T>(string methodName) : Attribute;
        internal sealed class OnFinalizer<T>(string methodName) : Attribute;
        internal sealed class OnTranspiler<T>(string methodName) : Attribute;
        internal sealed class OnVirtualPrefix<T>(string methodName) : Attribute;
        internal sealed class OnVirtualPostfix<T>(string methodName) : Attribute;
        internal sealed class OnVirtualFinalizer<T>(string methodName) : Attribute;
        internal sealed class OnVirtualTranspiler<T>(string methodName) : Attribute;
        """;
}
