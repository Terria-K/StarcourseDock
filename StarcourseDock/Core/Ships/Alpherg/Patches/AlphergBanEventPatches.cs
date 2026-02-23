using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed partial class AlphergBanEventPaches
{
    [HarmonyPatch(typeof(StoryNode), nameof(StoryNode.Filter))]
    [HarmonyPrefix]
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
