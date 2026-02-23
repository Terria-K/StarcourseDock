using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal partial class AlphergExclusiveArtifactPatches
{
    [HarmonyPatch(typeof(ArtifactReward), nameof(ArtifactReward.GetBlockedArtifacts))]
    [HarmonyPostfix]
    private static void GetBlockedArtifacts_Postfix(HashSet<Type> __result, State s)
    {
        if (s.ship.key == AlphergShip.AlphergEntry.UniqueName)
        {
            __result.Add(typeof(GlassCannon));
        }
    }
}
