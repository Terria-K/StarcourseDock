using HarmonyLib;
using Newtonsoft.Json;
using Riptide;

namespace Teuria.OnlineCoOp.TheClient;

public static class ClientMessages
{
    public static volatile bool hasShip;
    public static Ship enemyShip = null!;
    public static uint? seed;
    public static Deck? deck = null!;
    public static int playerCount = 2;
    public static int queuedVote = 0;
    public static AAfterPlayerTurn? storedTurn;

    [MessageHandler((ushort)PacketType.GetSeed)]
    public static void GetSeed(Message message)
    {
        seed = message.GetUInt();
    }

    [MessageHandler((ushort)PacketType.GetDeck)]
    public static void GetDeck(Message message)
    {
        deck = (Deck)message.GetInt();
    }

    [MessageHandler((ushort)PacketType.SinglePlayerAction)]
    public static void SinglePlayerAction(Message message)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };

        var data = JsonCompression.Decompress(message.GetBytes());

        var obj = (CardAction)JsonConvert.DeserializeObject(data, settings)!;
        if (MG.inst.g.state.route is Combat c)
        {
            c.Queue(obj);
        }
    }

    [MessageHandler((ushort)PacketType.MultiplayerAction)]
    public static void MultiplayerAction(Message message)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };

        var data = JsonCompression.Decompress(message.GetBytes());

        var obj = (CardAction)JsonConvert.DeserializeObject(data, settings)!;
        if (MG.inst.g.state.route is Combat c)
        {
            c.Queue(obj);
        }
    }

    [MessageHandler((ushort)PacketType.SetStatus)]
    public static void SetStatus(Message message)
    {
        var statusID = message.GetInt();
        var n = message.GetInt();
        var setMode = (StatusSetMode)message.GetUShort();

        switch (setMode)
        {
            case StatusSetMode.Add:
                MG.inst.g.state.ship.Add((Status)statusID, n);
                break;
            case StatusSetMode.Set:
                MG.inst.g.state.ship.Set((Status)statusID, n);
                break;
        }
    }

    [MessageHandler((ushort)PacketType.EndTurn)]
    public static void EndTurn(Message message)
    {
        queuedVote += 1;
    }
}