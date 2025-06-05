using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class AlphergExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key == AlphergShip.AlphergEntry.UniqueName)
        {
            __result.Add(typeof(GlassCannon));
        }
    }
}
