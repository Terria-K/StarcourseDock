using System.Runtime.CompilerServices;
using Nickel;

namespace Teuria.StarcourseDock;

public partial class ModEntry
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Patch(IHarmony harmony)
    {
        AAttack_Global_Patches.Patch(harmony);
    }
}
