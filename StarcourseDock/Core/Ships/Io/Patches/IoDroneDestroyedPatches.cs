using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class IoDroneDestroyedPatches : IPatchable
{
    [OnVirtualPostfix<StuffBase>(nameof(StuffBase.GetActionsOnDestroyed))]
    private static void StuffBase_GetActionsOnDestroyed_Postfix(
        State s, Combat c, StuffBase __instance)
    {
        if (s.HasArtifactFromColorless<GravitationalPull>())
        {
            var x = s.ship.GetWorldXOfPart("iocenter");
            if (x is {} y)
            {
                var dist = Math.Sign(__instance.x - y);
                c.QueueImmediate(new AIOSlurpObjectInstant() { thing = __instance, dist = dist, timer = 0.1 });
            }
        }
    }
}

