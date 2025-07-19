using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class WolfRayetExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key != WolfRayetShip.WolfRayetEntry.UniqueName)
        {
            __result.Add(typeof(HeatShieldV2));
            __result.Add(typeof(LaunchCodes));
            __result.Add(typeof(SeriousDedication));
        }
    }
}
