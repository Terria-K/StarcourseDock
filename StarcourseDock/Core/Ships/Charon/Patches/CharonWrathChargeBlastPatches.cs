using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class CharonWrathChargeBlastPatches 
{
    [HarmonyPatch(typeof(Ship), nameof(Ship.OnAfterTurn))]
    [HarmonyPrefix]
    public static void Ship_OnAfterTurn_Prefix(Ship __instance, State s, Combat c)
    {
        var wrath = __instance.Get(WrathChargeStatus.WrathCharge.Status);
        if (wrath > 0)
        {
            for (int i = 0; i < wrath; i += 1)
            {
                c.Queue(new WrathAction() { targetPlayer = __instance == c.otherShip });
            }
        }
    }

    [HarmonyPatch(typeof(AEnemyTurnAfter), nameof(AEnemyTurnAfter.Begin))]
    [HarmonyPostfix]
    public static void AEnemyTurnAfter_Begin_Postfix(State s, Combat c)
    {
        var hasSanity = s.HasArtifactFromColorless<SanityExpansion>();
        var wrath = s.ship.Get(WrathChargeStatus.WrathCharge.Status);

        int wrathRequired = hasSanity ? 2 : 1;

        if (wrath > wrathRequired)
        {
            var hasEnraged = s.HasArtifactFromColorless<EnrageDrill>();
            int wrathDamage = hasEnraged ? 2 : 1;
            for (int i = 0; i < wrath / (wrathRequired + 1); i += 1)
            {
                c.QueueImmediate(new WrathHurt() { hurtAmount = wrathDamage });
            }
        }

        s.ship.Set(WrathChargeStatus.WrathCharge.Status, 0);
    }
}