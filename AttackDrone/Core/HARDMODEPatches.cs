using HarmonyLib;

namespace Teuria.AttackDrone;

[HarmonyPatch]
public static class HARDMODEPatches
{
    [HarmonyPatch(typeof(HARDMODE), nameof(HARDMODE.OnReceiveArtifact))]
    [HarmonyPostfix]
    public static void OnReceiveArtifact_Postfix(State state)
    {
        if (state.ship.key == AttackDroneShip.ShipEntry.UniqueName)
        {
            state.ship.hull = 1;
            state.ship.hullMax = 1;
        }
    }
}
