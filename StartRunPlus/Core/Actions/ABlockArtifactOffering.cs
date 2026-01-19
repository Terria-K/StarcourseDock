namespace Teuria.StartRunPlus;

internal sealed class ABlockArtifactOffering : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        ModEntry.Instance.Helper.ModData.SetModData(s, "srpoffering", true);
    }
}
