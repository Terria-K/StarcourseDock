using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class IoDroneDestroyedPatches : IPatchable
{
    [OnPrefix<Combat>(nameof(Combat.DestroyDroneAt))]
    private static void Combat_DestroyDroneAt_Prefix(
        State s, Combat __instance, int x)
    {
        if (!s.HasArtifactFromColorless<GravitationalPull>())
        {
            return;
        }

        if (!__instance.stuff.TryGetValue(x, out var thing))
        {
            return;
        }

        var xcenter = s.ship.GetWorldXOfPart("iocenter");
        if (xcenter is { } y)
        {
            var dist = Math.Sign(thing.x - y);
            __instance.QueueImmediate(new AIOSlurpObjectInstant() { thing = thing, dist = dist, timer = 0.1 });
        }
    }
}

