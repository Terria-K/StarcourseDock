using CutebaltCore;

namespace Teuria.StarcourseDock;

internal sealed partial class GliesePartsPatches
{
    [OnPostfix<Part>(nameof(Part.GetBrokenPartSkin))]
    private static void Part_GetBrokenPartSkin_Postfix(Part __instance, ref string __result)
    {
        switch (__instance.key)
        {
            case "crystal1::StarcourseDock":
                __result = GlieseShip.GlieseScaffolding1.UniqueName;
                break;
            case "crystal2::StarcourseDock":
                __result = GlieseShip.GlieseScaffolding2.UniqueName;
                break;
            case "crystal3::StarcourseDock":
                __result = GlieseShip.GlieseScaffolding2.UniqueName;
                break;
            case "crystal4::StarcourseDock":
                __result = GlieseShip.GlieseScaffolding1.UniqueName;
                break;
        }
    }
}
