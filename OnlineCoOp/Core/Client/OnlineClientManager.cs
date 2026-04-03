using Riptide;

namespace Teuria.OnlineCoOp.TheClient;

public class ClientManager
{
    public static ClientManager Instance { get; private set; } = null!; 
    private Client? client;

    public Client? Client => client;


    public ClientManager()
    {
        Instance = this;
    }

    public void Connect(string port)
    {
        client = new Client();
        client.Connect(port);
    }

    public void Update()
    {
        client?.Update();
    }

    public void Send(Message message)
    {
        client?.Send(message);
    }
}

public class PlayerFighter : AI
{
    public int aiCounter;

    public override string Name()
    {
        return "CCD-19 \"CICADA\" Drone";
    }

    public override Ship BuildShipForSelf(State s)
    {
        character = new Character
        {
            type = "scrap"
        };
        return new Ship
        {
            x = 7,
            hull = 8,
            hullMax = 8,
            shieldMaxBase = 4,
            ai = this,
            chassisUnder = "chassis_cicada",
            parts = new List<Part>
            {
                new Part
                {
                    type = PType.wing,
                    skin = "wing_cicada",
                    key = "leftWing"
                },
                new Part
                {
                    type = PType.cockpit,
                    skin = "cockpit_cicada",
                    key = "cockpit"
                },
                new Part
                {
                    type = PType.cannon,
                    skin = "cannon_cicada",
                    key = "cannon"
                },
                new Part
                {
                    type = PType.missiles,
                    key = "missiles"
                },
                new Part
                {
                    type = PType.wing,
                    skin = "wing_cicada",
                    key = "rightWing",
                    flip = true
                }
            }
        };
    }

    public override void Update(G g, State s, Combat c, Ship ownShip) {}

    public override EnemyDecision PickNextIntent(State s, Combat c, Ship ownShip)
    {
        return new EnemyDecision();
    }
}