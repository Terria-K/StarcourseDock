using Nickel;

namespace Teuria.Utilities;

internal static class SeleneScaffoldManager
{
    public static Dictionary<string, IPartEntry> Scaffolds = [];

    public static void Add(string shipName, IPartEntry spriteEntry)
    {
        Scaffolds.Add(shipName, spriteEntry);
    }

    extension(IShipEntry entry)
    {
        public void AddSeleneScaffold(SeleneScaffoldConfiguration configuration)
        {
            Add(entry.UniqueName, configuration.Part);
        }
    }
}
