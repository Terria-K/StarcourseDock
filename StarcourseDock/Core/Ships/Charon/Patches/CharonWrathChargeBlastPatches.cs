using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class CharonWrathChargeBlastPatches : IPatchable
{
    [OnPrefix<AAfterPlayerTurn>(nameof(AAfterPlayerTurn.Begin))]
    public static void AAfterPlayerTurn_Begin_Prefix(State s, Combat c)
    {
        var wrath = s.ship.Get(WrathChargeStatus.WrathCharge.Status);
        if (wrath > 0)
        {
            for (int i = 0; i < wrath; i += 1)
            {
                c.Queue(new WrathAction());
            }
        }
    }

    [OnPostfix<AEnemyTurnAfter>(nameof(AEnemyTurnAfter.Begin))]
    public static void AEnemyTurnAfter_Begin_Postfix(State s, Combat c)
    {
        var wrath = s.ship.Get(WrathChargeStatus.WrathCharge.Status);

        if (wrath > 1)
        {
            c.QueueImmediate(new WrathHurt() { hurtAmount = wrath / 2 });
        }

        s.ship.Set(WrathChargeStatus.WrathCharge.Status, 0);
    }
}