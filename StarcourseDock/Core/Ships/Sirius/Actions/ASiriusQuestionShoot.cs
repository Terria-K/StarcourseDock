namespace Teuria.StarcourseDock;

internal sealed class ASiriusInquisitorShoot : CardAction
{
    public AAttack attackCopy = null!;

    public override void Begin(G g, State s, Combat c)
    {
        this.timer = 0.0;
        var attacks = new SortedList<int, CardAction>();
        foreach ((int x, StuffBase midRow) in c.stuff)
        {
            if (midRow is not SiriusSemiDualDrone)
            {
                continue;
            }


            AAttack copy = Mutil.DeepCopy(attackCopy);
            copy.fast = true;
            copy.fromX = null;
            copy.fromDroneX = x;
            copy.targetPlayer = midRow.targetPlayer;
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
            attacks.Add(x, copy);
        }
        c.QueueImmediate(attacks.Values);
    }
}
