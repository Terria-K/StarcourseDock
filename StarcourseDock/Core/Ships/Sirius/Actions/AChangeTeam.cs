namespace Teuria.StarcourseDock;

internal sealed class AChangeTeam : CardAction
{
    public SiriusSemiDualDrone? target;
    public bool targetPlayer;

    public override void Begin(G g, State s, Combat c)
    {
        if (target is null)
        {
            return;
        }

        target.targetPlayer = targetPlayer;
    }
}
