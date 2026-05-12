public struct GameSnapshot
{
    public int Turn;
    public uint Seed;

    public int Player1Health;
    public int Player2Health;

    public UnitState[] Player1Units;
    public UnitState[] Player2Units;
}

