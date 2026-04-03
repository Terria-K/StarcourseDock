using HarmonyLib;
using Riptide;
using Teuria.OnlineCoOp.TheClient;

namespace Teuria.OnlineCoOp;

[HarmonyPatch]
public static class AAfterPlayerTurnPatches
{
    [HarmonyPatch(typeof(AAfterPlayerTurn), nameof(AAfterPlayerTurn.Begin))]
    [HarmonyPrefix]
    public static bool AAfterPlayerTurn_Begin_Prefix(AAfterPlayerTurn __instance)
    {
        if (ClientMessages.queuedVote < ClientMessages.playerCount)
        {
            var message = Message.Create(MessageSendMode.Reliable, PacketType.EndTurn);
            ClientManager.Instance.Send(message);
            
            ClientMessages.storedTurn = __instance;
            return false;
        }

        ClientMessages.queuedVote = 0;

        return true;
    }
}
