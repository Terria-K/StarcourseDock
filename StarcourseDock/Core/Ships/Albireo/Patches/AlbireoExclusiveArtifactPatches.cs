using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AlbireoExclusiveArtifactPatches : IPatchable
{
    [OnPostfix<ArtifactReward>(nameof(ArtifactReward.GetBlockedArtifacts))]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key != AlbireoShip.AlbireoShipEntry.UniqueName)
        {
            __result.Add(typeof(PolarityWings));
        }
    }
}