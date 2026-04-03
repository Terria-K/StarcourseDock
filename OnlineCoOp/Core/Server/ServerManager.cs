using Riptide;
using Riptide.Utils;

namespace Teuria.OnlineCoOp.TheServer;

public record class ShipID(ushort ClientID, int ID);

public class ServerManager
{
    public static int IncrementID;
    public static Stack<int> PlayerIDs = [];

    private static Dictionary<ushort, ShipID> clientShip = [];

    public static IReadOnlyCollection<ushort> Clients => clientShip.Keys;

    public static ServerManager Instance { get; private set; } = null!;
    public bool HasStarted { get => hasStarted; set => hasStarted = value; }
    public Server? Server { get => server; set => server = value; }
    public ushort Port { get => port; set => port = value; }

    public List<Deck> DecksAvailable = [];

    private Server? server;
    private ushort port;
    private volatile bool hasStarted;

    public ServerManager()
    {
        Instance = this;
    }

    public void AddClient(ushort clientID, ShipID id)
    {
        clientShip[clientID] = id;
    }

    public void RemoveClient(ushort clientID)
    {
        clientShip.Remove(clientID);
    }

    public ShipID GetClient(ushort clientID)
    {
        return clientShip[clientID];
    }

    public void Create(ushort port)
    {
        server = new Server();
        server.ClientConnected += OnClientConnected;
        server.ClientDisconnected += OnClientDisconnected;
        this.port = port;
    }

    public void Start()
    {
        new Thread(Loop).Start();
    }

    public void Stop()
    {
        server?.Stop();
        hasStarted = false;
    }

    public void Loop()
    {
        if (server is null)
        {
            return;
        }

        server.Start(port, 3);
        hasStarted = true;

        while (hasStarted)
        {
            server.Update();
            Thread.Sleep(10);
        }
    }

    public void Send(Message message, ushort fromClientId)
    {
        server?.Send(message, fromClientId);
    }

    public void SendToAll(Message message)
    {
        server?.SendToAll(message);
    }


    static void OnClientConnected(object? sender, ServerConnectedEventArgs e)
    {
        RiptideLogger.Log(LogType.Info, $"Client {e.Client.Id} is connected!");
        int id;

        if (!PlayerIDs.TryPop(out id))
        {
            id = IncrementID;
            IncrementID += 1;
        }

        Instance.AddClient(e.Client.Id, new ShipID(e.Client.Id, id));
    }

    static void OnClientDisconnected(object? sender, ServerDisconnectedEventArgs e)
    {
        RiptideLogger.Log(LogType.Info, $"Client {e.Client.Id} has been disconnected!");

        var data = clientShip[e.Client.Id];

        PlayerIDs.Push(data.ID);

        Instance.RemoveClient(e.Client.Id);
    }
}
