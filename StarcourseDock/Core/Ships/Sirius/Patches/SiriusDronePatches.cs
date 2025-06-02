using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class SiriusDronePatches : IPatchable
{
    [OnPrefix<AJupiterShoot>(nameof(AJupiterShoot.Begin))]
    private static void AJupiterShoot_Begin_Prefix(AJupiterShoot __instance, State s, Combat c)
    {
        bool damageUpgraded = false;
        SortedList<int, CardAction> siriusAttacks = [];
        foreach ((int x, StuffBase midRow) in c.stuff)
        {
            if (midRow is not SiriusDrone sirius)
            {
                continue;
            }

            if (sirius.upgrade == Upgrade.A && !damageUpgraded)
            {
                __instance.attackCopy.damage += 1;
                damageUpgraded = true;
            }

            int partX = s.ship.parts.FindIndex(p => p.type == PType.comms);
            if (partX >= 0 && midRow.x == partX + s.ship.x)
            {
                continue;
            }

            AAttack copy = Mutil.DeepCopy<AAttack>(__instance.attackCopy);
            copy.fast = true;
            copy.fromX = null;
            copy.fromDroneX = midRow.x;
            copy.targetPlayer = !midRow.targetPlayer;
            copy.shardcost = 0;
            int beforeDamage = copy.damage;
            foreach (Artifact r in s.EnumerateAllArtifacts())
            {
                copy.damage += r.ModifyBaseJupiterDroneDamage(s, c, midRow);
                if (copy.damage > beforeDamage)
                {
                    copy.artifactPulse = r.Key();
                    beforeDamage = copy.damage;
                }
            }
            siriusAttacks.Add(midRow.x, copy);
        }
        c.QueueImmediate(siriusAttacks.Values);
    }
}
