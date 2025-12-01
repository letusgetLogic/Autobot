[System.Serializable]
public struct Attribute
{
    public int HP; // hit point
    public int ATK; // attack point
    public int Energy;

    public Attribute(int hp, int atk)
    {
        HP = hp;
        ATK = atk;
        Energy = 0;
    }

    public Attribute(int hp, int atk, int energy)
    {
        HP = hp;
        ATK = atk;
        Energy = energy;
    }
}

