namespace Teuria.StarcourseDock;

internal sealed class AChangeTeam : CardAction
{
    public SiriusSemiDualDrone? target;

    public override void Begin(G g, State s, Combat c)
    {
        if (target is null)
        {
            return;
        }

        target.targetPlayer = !target.targetPlayer;
    }
}
