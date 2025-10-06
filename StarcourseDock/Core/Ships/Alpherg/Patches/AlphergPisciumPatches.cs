using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AlphergPisciumPatches : IPatchable
{

    [OnPrefix<AAttack>(nameof(AAttack.Begin))]
    private static void AAttack_Begin_Prefix(AAttack __instance)
    {
        Piscium.aAttack = __instance;
    } 

    [OnFinalizer<AAttack>(nameof(AAttack.Begin))]
    private static void AAttack_Begin_Finalizer()
    {
        Piscium.aAttack = null;
    } 

    [OnPostfix<AVolleyAttackFromAllCannons>(nameof(AVolleyAttackFromAllCannons.Begin))]
    private static void AVolleyAttackFromAllCannons_Begin_Postfix(
        AVolleyAttackFromAllCannons __instance,
        State s,
        Combat c
    )
    {
        var piscium = s.GetArtifactFromColorless<Piscium>();
        if (piscium is null)
        {
            return;
        }

        piscium.isRight = !piscium.isRight;

        int idx = c.cardActions.FindIndex(x => x.GetType() == typeof(AJupiterShoot));
        if (idx == -1)
        {
            c.QueueImmediate(new ASwapScaffold() { isRight = piscium.isRight });
            return;
        }

        c.cardActions.Insert(idx, new ASwapScaffold() { isRight = piscium.isRight });
    }
}
