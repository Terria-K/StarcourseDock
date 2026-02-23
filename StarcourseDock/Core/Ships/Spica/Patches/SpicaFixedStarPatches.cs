using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class SpicaFixedStarPatches
{
    [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
    [HarmonyPrefix]
    private static bool AMove_Begin_Prefix(AMove __instance, State s, Combat c)
    {
        if (
            s.HasArtifactFromColorless<FixedStar>()
            && __instance.targetPlayer
            && __instance.fromEvade
        )
        {
            __instance.timer = 0f;
            c.QueueImmediate(
                new ACannonMove()
                {
                    dir = __instance.dir,
                    ignoreHermes = __instance.ignoreHermes,
                    preferRightWhenZero = __instance.preferRightWhenZero,
                }
            );
            return false;
        }

        return true;
    }
}
