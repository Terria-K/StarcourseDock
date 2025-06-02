using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class GlieseStunPatches : IPatchable
{
    [OnPrefix<AStunPart>(nameof(AStunPart.Begin))]
    private static void AStunPart_Begin_Prefix(AStunPart __instance, State s, Combat c)
    {
        FrostCannon? frostCannon = s.GetArtifact<FrostCannon>();
        if (frostCannon is null)
        {
            return;
        }

        Part? part = c.otherShip.GetPartAtWorldX(__instance.worldX);
        if (part != null && part.stunModifier != PStunMod.unstunnable)
        {
            if (part.intent != null)
            {
                c.Queue(
                    new AStatus()
                    {
                        targetPlayer = false,
                        status = ColdStatus.ColdEntry.Status,
                        statusAmount = 1,
                    }
                );

                frostCannon.Pulse();
            }
        }
    }
}
