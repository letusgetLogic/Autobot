
[System.Serializable]
public struct Attribute
{
    /// <summary>
    /// Representation of hit point.
    /// </summary>
    public int HP;

    /// <summary>
    /// Representation of attack point.
    /// </summary>
    public int ATK;

    /// <summary>
    /// Representation of energy point.
    /// </summary>
    public int ENG;

    public bool HasValue;

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
        HasValue = true;
    }

    /// <summary>
    /// Constructor of Attribute with given parameter.
    /// </summary>
    /// <param name="_hp"></param>
    /// <param name="_atk"></param>
    /// <param name="_energy"></param>
    public Attribute(int _hp, int _atk, int _energy)
    {
        HP = _hp;
        ATK = _atk;
        ENG = _energy;
        HasValue = true;
    }

    /// <summary>
    /// Is one of the attributes greater than 0?
    /// </summary>
    /// <returns></returns>
    public bool IsGreaterThan0()
    {
        return HP > 0 || ATK > 0 || ENG > 0;
    }
}

