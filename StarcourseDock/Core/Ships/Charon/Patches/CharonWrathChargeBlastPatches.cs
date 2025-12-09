using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class CharonWrathChargeBlastPatches : IPatchable
{
    [OnPrefix<Ship>(nameof(Ship.OnAfterTurn))]
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

    [OnPostfix<AEnemyTurnAfter>(nameof(AEnemyTurnAfter.Begin))]
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