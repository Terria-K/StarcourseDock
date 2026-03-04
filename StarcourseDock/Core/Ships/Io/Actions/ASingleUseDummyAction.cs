namespace Teuria.StarcourseDock;

internal sealed class ASingleUseDummyAction : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        if (!s.HasArtifactFromColorless<TrashHatch>())
        {
            return;
        }

        c.QueueImmediate(new AStatus() { status = ReoxidoStatus.Reoxido.Status, statusAmount = 1, targetPlayer = true });
    }

    public override Icon? GetIcon(State s) => new Icon(
        s.HasArtifactFromColorless<TrashHatch>() ? Sprites.icons_poisonedTrash.Sprite : StableSpr.icons_singleUse, 
        null, 
        Colors.white
    );

    public override List<Tooltip> GetTooltips(State s)
    {
        if (!s.HasArtifactFromColorless<TrashHatch>())
        {
            return [];
        }

        return StatusMeta.GetTooltips(ReoxidoStatus.Reoxido.Status, 4);
    }
}
