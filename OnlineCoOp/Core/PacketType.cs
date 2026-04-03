namespace Teuria.OnlineCoOp;

public enum PacketType : ushort
{
    PlayerSelection = 1,
    GetSeed,
    GetDeck,
    SinglePlayerAction,
    MultiplayerAction,
    SetStatus,
    EndTurn
}

public enum StatusSetMode : ushort
{
    Add,
    Set
}
