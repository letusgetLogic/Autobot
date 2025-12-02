[System.Serializable]
public struct Attribute
{
    public int HP; // hit point
    public int ATK; // attack point
    public int ENG; // energy point

    /// <summary>
    /// Constructor of Attribute with given parameter and 0 energy.
    /// </summary>
    /// <param name="_hp"></param>
    /// <param name="_atk"></param>
    public Attribute(int _hp, int _atk)
    {
        HP = _hp;
        ATK = _atk;
        ENG = 0;
    }

    /// <summary>
    /// Constructor of Attribute with given parameter.
    /// </summary>
    /// <param name="_hp"></param>
    /// <param name="_atk"></param>
    /// <param name="_eng"></param>
    public Attribute(int _hp, int _atk, int _eng)
    {
        HP = _hp;
        ATK = _atk;
        ENG = _eng;
    }
}

