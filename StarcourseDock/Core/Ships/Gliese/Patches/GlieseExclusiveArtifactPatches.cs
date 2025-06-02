using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class GlieseExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key != GlieseShip.GlieseEntry.UniqueName)
        {
            __result.Add(typeof(CrystalCoreV2));
        }
        if (s.ship.key == GlieseShip.GlieseEntry.UniqueName)
        {
            __result.Add(typeof(StunCalibrator));
        }
    }
}
