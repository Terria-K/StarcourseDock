using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Newtonsoft.Json;
using Riptide;
using Teuria.OnlineCoOp.TheClient;

namespace Teuria.OnlineCoOp;

[HarmonyPatch]
public static class CombatPatches
{
    public static bool playingCard;
    public static HashSet<Type> singlePlayerActions = [
        typeof(AAddCard),
        typeof(ADrawCard),
        typeof(ADiscard),
        typeof(APlayOtherCard),
        typeof(AReverseHand),
        typeof(AShuffleHand)
    ];

    [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static void Combat_TryPlayCard_Prefix()
    {
        playingCard = true;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.QueueImmediate), typeof(IEnumerable<CardAction>))]
    [HarmonyPrefix]
    public static bool Combat_QueueImmediateA_Postfix(IEnumerable<CardAction> a)
    {
        if (!playingCard)
        {
            return true;
        }

        TrySend(a);
        return false;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.Queue), typeof(IEnumerable<CardAction>))]
    [HarmonyPrefix]
    public static bool Combat_QueueA_Postfix(IEnumerable<CardAction> actions)
    {
        if (!playingCard)
        {
            return true;
        }

        TrySend(actions);
        return false;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.QueueImmediate), typeof(CardAction))]
    [HarmonyPrefix]
    public static bool Combat_QueueImmediate_Postfix(CardAction a)
    {
        if (!playingCard)
        {
            return true;
        }

        TrySend(a);
        return false;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.Queue), typeof(CardAction))]
    [HarmonyPrefix]
    public static bool Combat_Queue_Postfix(CardAction action)
    {
        if (!playingCard)
        {
            return true;
        }

        TrySend(action);

        return false;
    }

    private static void TrySend(IEnumerable<CardAction> action)
    {
        if (action is null)
        {
            return;
        }

        foreach (var a in action)
        {
            TrySend(a);
        }
    }

    private static void TrySend(CardAction action)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };

        var compressedJson = JsonCompression.Compress(JsonConvert.SerializeObject(action, settings));

        var type = action.GetType();

        Message message;
        if (singlePlayerActions.Contains(type))
        {
            message = Message.Create(MessageSendMode.Reliable, PacketType.SinglePlayerAction);
        }
        else
        {
            message = Message.Create(MessageSendMode.Reliable, PacketType.MultiplayerAction);
        }

        message.AddBytes(compressedJson);

        ClientManager.Instance.Send(message);
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.TryPlayCard))]
    [HarmonyPostfix]
    public static void Combat_TryPlayCard_Postfix()
    {
        playingCard = false;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.Update))]
    [HarmonyPostfix]
    public static void Combat_Update_Postfix(Combat __instance)
    {
        if (ClientMessages.queuedVote < ClientMessages.playerCount)
        {
            return;
        }

        if (ClientMessages.storedTurn is null)
        {
            return;
        }

        __instance.QueueImmediate(ClientMessages.storedTurn);
        ClientMessages.storedTurn = null;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.PlayerCanAct))]
    [HarmonyPostfix]
    public static void Combat_PlayerCanAct_Postfix(ref bool __result)
    {
        __result = __result && ClientMessages.storedTurn == null;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.DoEvade))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static void Combat_DoEvade_Prefix(Combat __instance, G g)
    {
        playingCard = true;

        if (__instance.PlayerCanAct(g.state))
        {
            bool anchored = Enumerable.Any(Enumerable.OfType<TrashAnchor>(__instance.hand));
            bool lockdowned = g.state.ship.Get(Status.lockdown) > 0;

            if (g.state.ship.Get(Status.evade) <= 0 || anchored || lockdowned)
            {
                return;
            }

            var message = Message.Create(MessageSendMode.Reliable, PacketType.SetStatus);
            message.AddInt((int)Status.evade);
            message.AddInt(-1);
            message.AddUShort((ushort)StatusSetMode.Add);
            ClientManager.Instance.Send(message);
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.DoEvade))]
    [HarmonyPostfix]
    public static void Combat_DoEvade_Postfix()
    {
        playingCard = false;
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.DoDroneShift))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static void Combat_DoDroneShift_Prefix(Combat __instance, G g)
    {
        playingCard = true;
        if (__instance.PlayerCanAct(g.state) && g.state.ship.Get(Status.droneShift) > 0)
        {
            var message = Message.Create(MessageSendMode.Reliable, PacketType.SetStatus);
            message.AddInt((int)Status.droneShift);
            message.AddInt(-1);
            message.AddUShort((ushort)StatusSetMode.Add);
            ClientManager.Instance.Send(message);           
        }
    }

    [HarmonyPatch(typeof(Combat), nameof(Combat.DoDroneShift))]
    [HarmonyPostfix]
    public static void Combat_DoDroneShift_Postfix()
    {
        playingCard = false;
    }
}
