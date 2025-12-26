using CutebaltCore;

namespace Teuria.StarcourseDock;

internal partial class WolfRayetMissilesPatches : IPatchable
{
    [OnPostfix<Ship>(nameof(Ship.OnAfterTurn))]
    private static void OnAfterTurn_Postfix(Ship __instance, Combat c)
    {
        if (__instance.GetPartTypeCount(WolfRayetShip.MissilePartType.PartType) == 0)
        {
            return;
        }

        if (__instance.Get(Status.heat) >= __instance.heatTrigger)
        {
            c.Queue(new ALaunchMissiles() { isPlayerShip = __instance.isPlayerShip });
        }
    }

    [OnPrefix<Ship>(nameof(Ship.NormalDamage))]
    private static void Ship_NormalDamage_Prefix(Ship __instance, int? maybeWorldGridX)
    {
        if (maybeWorldGridX is null)
        {
            return;
        }

        Part? part = __instance.GetPartAtWorldX(maybeWorldGridX.Value);

        if (part == null)
        {
            return;
        }

        if (part.type == WolfRayetShip.MissilePartType.PartType && !part.active)
        {
            part.active = true;
        }
    }
}
