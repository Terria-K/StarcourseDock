using FMOD;
using FSPRO;

namespace Teuria.StarcourseDock;

public sealed class AFragile : CardAction
{
	public int worldX;
	public bool targetPlayer;
	public bool justTheActiveOverride;

    public override void Begin(G g, State s, Combat c)
    {
		timer *= 0.5;
		Part? part = (targetPlayer ? s.ship : c.otherShip).GetPartAtWorldX(worldX);
		if (part != null)
		{
			if (justTheActiveOverride)
			{
				part.damageModifierOverrideWhileActive = CharonShip.Fragile.PartDamageModifier;
			}
			else
			{
				part.damageModifier = CharonShip.Fragile.PartDamageModifier;
			}

			Audio.Play(new GUID?(Event.Status_PowerDown), true);
			return;
		}
		timer = 0.0;
    }
}