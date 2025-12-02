namespace Teuria.StarcourseDock;

internal sealed class ASingleUseDummyAction : CardAction
{
    public override Icon? GetIcon(State s) => new Icon(StableSpr.icons_singleUse, null, Colors.white);
}
