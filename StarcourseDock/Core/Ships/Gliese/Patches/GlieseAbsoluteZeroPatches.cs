using CutebaltCore;
using ZLinq;

namespace Teuria.StarcourseDock;

internal sealed partial class GlieseAbsoluteZeroPatches : IPatchable
{
    [OnPostfix<Card>(nameof(Card.GetActualDamage))]
    private static void Card_GetActualDamage_Postfix(State s, bool targetPlayer, ref int __result)
    {
        if (targetPlayer)
        {
            return;
        }

        Combat? combat = s.route as Combat;

        if (combat == null)
        {
            return;
        }

        if (combat.hand.AsValueEnumerable().OfType<AbsoluteZero>().Any())
        {
            __result = 0;
        }
    }
}
