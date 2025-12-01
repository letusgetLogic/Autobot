[System.Serializable]
public struct Attribute
{
    public int HP; // hit point
    public int ATK; // attack point
    public int ENG; // energy point

    public Attribute(int _hp, int _atk)
    {
        HP = _hp;
        ATK = _atk;
        ENG = 0;
    }

    public Attribute(int _hp, int _atk, int _eng)
    {
        HP = _hp;
        ATK = _atk;
        ENG = _eng;
    }
}

