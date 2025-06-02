using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class SpicaExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key != SpicaShip.SpicaEntry.UniqueName)
        {
            __result.Add(typeof(ShrinkMechanismV2));
            __result.Add(typeof(TinyWormhole));
        }
    }
}
