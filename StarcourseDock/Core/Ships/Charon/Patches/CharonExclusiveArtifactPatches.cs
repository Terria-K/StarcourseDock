using System.Runtime.CompilerServices;
using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class CharonExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key == CharonShip.CharonEntry.UniqueName)
        {
            __result.Add(typeof(GlassCannon));
        }
    }
}
