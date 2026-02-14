using System.Runtime.CompilerServices;

[System.Serializable]
public struct Attribute
{
    //
    // Summary:
    //     Representation of hit point.
    public int HP;

    //
    // Summary:
    //    Representation of attack point.
    public int ATK;

    //
    // Summary:
    //     Represents energy point.
    public int ENG;


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
    /// <param name="_energy"></param>
    public Attribute(int _hp, int _atk, int _energy)
    {
        HP = _hp;
        ATK = _atk;
        ENG = _energy;
    }

    /// <summary>
    /// Is the struct default
    /// </summary>
    /// <returns></returns>
    public bool HasValue()
    {
        return HP > 0 || ATK > 0 || ENG > 0;
    }
}

