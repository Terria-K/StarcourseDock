namespace Teuria.StarcourseDock;

internal sealed class ALaunchMissiles : CardAction
{
    public bool targetPlayer;
    public override void Begin(G g, State s, Combat c)
    {
        timer = 1;
        Ship target = (targetPlayer ? s.ship : c.otherShip);

        for (int i = 0; i < target.parts.Count; i++)
        {
            Part part = target.parts[i];
            if (part == null)
            {
                continue;
            }

            if (part.type == WolfRayetShip.MissilePartType && part.active)
            {
                c.Queue(new ALaunchMissile()
                {
                    part = part,
                    localX = i,
                    targetPlayer = targetPlayer
                });
            }
        }
        timer = 0;
    }
}

internal sealed class ALaunchMissile : CardAction
{
    public bool targetPlayer;
    public Part? part;
    public int localX;

    public override void Begin(G g, State s, Combat c)
    {
        Ship target = (!targetPlayer ? s.ship : c.otherShip);
        RaycastResult ray = CombatUtils.RaycastFromShipLocal(s, c, localX, !targetPlayer);

        if (part != null)
        {
            c.fx.Add(new MissileLaunchFX()
            {
                part = part,
                hitShip = ray.hitShip,
                hitDrone = ray.hitDrone,
                worldX = (localX + s.ship.x) * 16
            });

            part.skin = WolfRayetShip.MissileEmptySlot.UniqueName;
            part.type = PType.empty;
            part.stunModifier = PStunMod.none;
            part.key = $"{ModEntry.Instance.Package.Manifest.UniqueName}::LaunchedMissile";
            part.active = false;
        }

        DamageDone dmg = new DamageDone();

        if (ray.hitDrone)
        {
            var invincible = c.stuff[ray.worldX].Invincible();

            foreach (var artifact in s.EnumerateAllArtifacts())
            {
                if (artifact.ModifyDroneInvincibility(s, c, c.stuff[ray.worldX]) == true)
                {
                    invincible = true;
                    artifact.Pulse();
                }
            }

            if (c.stuff[ray.worldX].bubbleShield)
            {
                c.stuff[ray.worldX].bubbleShield = false;
            }
            else if (invincible)
            {
                c.QueueImmediate(c.stuff[ray.worldX].GetActionsOnShotWhileInvincible(s, c, true, 4));
            }
            else
            {
                c.DestroyDroneAt(s, ray.worldX, true);
            }

            EffectSpawner.NonCannonHit(g, false, ray, dmg);
        }

        if (ray.hitShip)
        {
            dmg = target.NormalDamage(s, c, 4, ray.worldX, false);
            Part? partAtWorldX = target.GetPartAtWorldX(ray.worldX);

            if (partAtWorldX != null)
            {
                if (partAtWorldX.stunModifier == PStunMod.stunnable)
                {
                    c.QueueImmediate(new AStunPart() { worldX = ray.worldX });
                }
            }

            if ((target.Get(Status.payback) > 0) || target.Get(Status.tempPayback) > 0)
            {
                c.QueueImmediate(
                    new AAttack()
                    {
                        paybackCounter = 1,
                        damage = Card.GetActualDamage(s, target.Get(Status.payback) + target.Get(Status.tempPayback), true),
                        targetPlayer = targetPlayer,
                        fast = true,
                        storyFromPayback = targetPlayer
                    }
                );
            }

            if (target.Get(Status.reflexiveCoating) >= 1)
            {
                c.QueueImmediate(new AArmor()
                {
                    worldX = ray.worldX,
                    targetPlayer = !targetPlayer,
                    justTheActiveOverride = true
                });
            }

            EffectSpawner.NonCannonHit(g, false, ray, dmg);
        }
    }
}
