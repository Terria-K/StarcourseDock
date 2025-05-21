using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class DoubleStarAction : IEvadeAction
{
    public bool CanDoEvadeAction(IEvadeAction.ICanDoEvadeArgs args)
    {
        var doubleStar = args.State.GetArtifact<DoubleStar>();
        if (doubleStar is null || doubleStar.binaryStarDetected)
        {
            return false;
        }

        return true;
    }

    public IReadOnlyList<CardAction> ProvideEvadeActions(IEvadeAction.IProvideEvadeActionsArgs args)
    {
        var doubleStar = args.State.GetArtifact<DoubleStar>();
        if (doubleStar is null || doubleStar.binaryStarDetected)
        {
            return [];
        }

        return [
            new AMove() 
            {
                dir = args.Direction == Direction.Left ? -1 : 1,
                targetPlayer = true
            },
            new ACheckParts()
        ];
    }
}
