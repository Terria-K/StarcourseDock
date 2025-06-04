using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class SiriusExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key != SiriusShip.SiriusEntry.UniqueName)
        {
            __result.Add(typeof(SiriusSubwoofer));
            __result.Add(typeof(SiriusMissileBayV2));
            __result.Add(typeof(SiriusInquisitor));
        }
    }
}
