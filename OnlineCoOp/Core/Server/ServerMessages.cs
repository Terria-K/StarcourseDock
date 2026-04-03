using Riptide;

namespace Teuria.OnlineCoOp.TheServer;

public static class ServerMessages
{
    [MessageHandler((ushort)PacketType.PlayerSelection)]
    private static void PlayerSelection(ushort fromClientId, Message message)
    {
        ServerManager.Instance.DecksAvailable.Clear();

        var length = message.GetInt();
        for (int i = 0; i < length; i += 1)
        {
            var c = message.GetInt();
            ServerManager.Instance.DecksAvailable.Add((Deck)c);
        }
    }

    [MessageHandler((ushort)PacketType.GetSeed)]
    private static void GetSeed(ushort fromClientId, Message message)
    {
        var seedMessage = Message.Create(MessageSendMode.Reliable, (ushort)PacketType.GetSeed);
        seedMessage.AddUInt(2801942);
        ServerManager.Instance.Send(seedMessage, fromClientId);
    }

    [MessageHandler((ushort)PacketType.GetDeck)]
    private static void GetDeck(ushort fromClientId, Message message)
    {
        var client = ServerManager.Instance.GetClient(fromClientId);
        var deck = ServerManager.Instance.DecksAvailable[client.ID];
        var deckMessage = Message.Create(MessageSendMode.Reliable, (ushort)PacketType.GetDeck);
        deckMessage.AddInt((int)deck);
        ServerManager.Instance.Send(deckMessage, fromClientId);
    }

    [MessageHandler((ushort)PacketType.SinglePlayerAction)]
    private static void SinglePlayerAction(ushort fromClientId, Message message)
    {
        ServerManager.Instance.Send(message, fromClientId);
    }

    [MessageHandler((ushort)PacketType.MultiplayerAction)]
    private static void MultiplayerAction(ushort fromClientId, Message message)
    {
        ServerManager.Instance.SendToAll(message);
    }

    [MessageHandler((ushort)PacketType.SetStatus)]
    private static void SetStatus(ushort fromClientId, Message message)
    {
        foreach (var clientID in ServerManager.Clients)
        {
            if (fromClientId != clientID)
            {
                ServerManager.Instance.Send(message, clientID);
                return;
            }
        }
    }

    [MessageHandler((ushort)PacketType.EndTurn)]
    private static void EndTurn(ushort fromClientId, Message message)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, PacketType.EndTurn);
        ServerManager.Instance.SendToAll(msg);
    }
}