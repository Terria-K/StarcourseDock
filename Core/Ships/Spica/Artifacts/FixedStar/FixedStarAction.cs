using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class FixedStarAction : IEvadeAction
{
    public bool CanDoEvadeAction(IEvadeAction.ICanDoEvadeArgs args)
    {
        return true;
    }

    public IReadOnlyList<CardAction> ProvideEvadeActions(IEvadeAction.IProvideEvadeActionsArgs args)
    {
        if (!args.State.EnumerateAllArtifacts().Any(x => x is FixedStar))
        {
            return [];
        }
        return [
            new ACannonMove() 
            {
                dir = args.Direction == Direction.Left ? -1 : 1
            }
        ];
    }
}
