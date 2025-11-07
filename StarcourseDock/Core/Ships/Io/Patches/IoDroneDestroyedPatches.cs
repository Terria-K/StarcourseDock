using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class IoDroneDestroyedPatches : IPatchable
{
    [OnPrefix<Combat>(nameof(Combat.DestroyDroneAt))]
    private static void Combat_DestroyDroneAt_Prefix(
        State s, Combat __instance, int x)
    {
        var gravPull = s.GetArtifactFromColorless<GravitationalPull>();
        if (gravPull is not null)
        {
            if (!__instance.stuff.TryGetValue(x, out var thing))
            {
                return;
            }

            var xcenter = s.ship.GetWorldXOfPart("iocenter");
            if (xcenter is { } y)
            {
                gravPull.Pulse();
                var dist = Math.Sign(thing.x - y);
                __instance.QueueImmediate(new AIOSlurpObjectInstant() 
                { 
                    thing = thing, 
                    dist = dist, timer = 0.1, 
                    aUpgrade = false 
                });
            }

            return;
        }

        var gravPullV2 = s.GetArtifactFromColorless<GravitationalPullV2>();
        if (gravPullV2 is not null)
        {
            if (!__instance.stuff.TryGetValue(x, out var thing))
            {
                return;
            }

            var xcenter = s.ship.GetWorldXOfPart("iocenter");
            if (xcenter is { } y)
            {
                gravPullV2.Pulse();
                var dist = Math.Sign(thing.x - y);
                __instance.QueueImmediate(new AIOSlurpObjectInstant() 
                { 
                    thing = thing, 
                    dist = dist, timer = 0.1, 
                    aUpgrade = false 
                });
            }

            return;
        }
    }
}

