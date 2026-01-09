namespace Teuria.StarcourseDock;

internal sealed class ADummyActionButHasTooltip : CardAction
{
    public override void Begin(G g, State s, Combat c) => timer = 0.0f;

    public override List<Tooltip> GetTooltips(State s)
    {
        return [
            ..StatusMeta.GetTooltips(Status.engineStall, 3)
        ];
    }
    
}