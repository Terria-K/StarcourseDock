using static Shockah.Kokoro.IKokoroApi.IV2.IEvadeHookApi;

namespace Teuria.StarcourseDock;

internal class FixedStarHook : IHook
{
    public bool IsEvadeActionEnabled(IHook.IIsEvadeActionEnabledArgs args) 
    {
        var state = args.State;

        if (!state.EnumerateAllArtifacts().Any(x => x is FixedStar))
        {
            return true;
        }

        int dir = (int)args.Direction;
        int cannonIndex = 0;
        int i = 0;
        int length = state.ship.parts.Count;
        foreach (var s in state.ship.parts)
        {
            if (s.key == "closeToScaffold")
            {
                cannonIndex = i;
            }
            i += 1;
        }
        if (dir == -1 && cannonIndex == 1)
        {
            return false;
        }

        if (dir == 1 && cannonIndex == length - 2)
        {
            return false;
        }

        return true;
    }
}