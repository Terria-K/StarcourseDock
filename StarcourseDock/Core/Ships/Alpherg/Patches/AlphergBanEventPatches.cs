using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class AlphergBanEventPaches : IPatchable
{
    [OnPrefix<StoryNode>(nameof(StoryNode.Filter))]
    internal static bool StoryNode_Filter_Prefix(string key, State s, ref bool __result)
    {
        if (s.ship.key == AlphergShip.AlphergEntry.UniqueName && key == "AddScaffold")
        {
            __result = false;
            return false;
        }

        return true;
    }
}
