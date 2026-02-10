using System;

/// <summary>
/// Data structure to save unit data between phases and sessions.
/// </summary>
[Serializable]
public struct SaveUnitData
{
    public bool HasReference { get; set; }
    public string ID { get; set; }
    public int Index { get; set; }
    public UnitType UnitType { get; set; }
    public Attribute Max;
    public int MaxXP { get; set; }

    /// <summary>
    /// The current attributes, which are displayed.
    /// </summary>
    public readonly Attribute Cur => current;
    private Attribute current;

    /// <summary>
    /// The basis attributres, which scale up from merging.
    /// </summary>
    public readonly Attribute Basis => basis;
    private Attribute basis;

    /// <summary>
    /// The buff attributes, which scale up from abilities only during shop phase, are pernament.
    /// </summary>
    public readonly Attribute Buff => buff;
    private Attribute buff;

    /// <summary>
    /// The temporary buff attributes, which scale up from abilities, 
    /// was setted to 0 at the end of battle phase.
    /// </summary>
    public readonly Attribute TempBuff => temporaryBuff;
    private Attribute temporaryBuff;

    public readonly int FullHP => Basis.HP + Buff.HP;
    public readonly int FullATK => Basis.ATK + Buff.ATK;

    public readonly int XP => xp;
    private int xp;

    public int Durability { get; set; }
    public float DurabilityRatio { get; set; }

    public UnitState UnitState { get; set; }

    public bool IsTeamLeft { get; set; }

    /// <summary>
    /// Sets the current hit points and update the repair panel, if needed.
    /// </summary>
    /// <param name="_hp"></param>
    /// <param name="_updateRepair"></param>
    public void SetHP(int _hp, Action _updateRepair)
    {
        if (_hp > Max.HP)
            current.HP = Max.HP;
        else
            current.HP = _hp;

        _updateRepair?.Invoke();
    }

    /// <summary>
    /// Sets the current attack points.
    /// </summary>
    /// <param name="_atk"></param>
    public void SetATK(int _atk)
    {
        if (_atk > Max.ATK)
            current.ATK = Max.ATK;
        else
            current.ATK = _atk;
    }

    /// <summary>
    /// Sets the current energy.
    /// </summary>
    /// <param name="_energy"></param>
    public void SetEnergy(int _energy)
    {
        if (_energy > Max.ENG)
            current.ENG = Max.ENG;
        else
            current.ENG = _energy;
    }

    /// <summary>
    /// Sets experience points.
    /// </summary>
    /// <param name="_xp"></param>
    public void SetXP(int _xp)
    {
        if (_xp > MaxXP)
            xp = MaxXP;
        else
            xp = _xp;
    }

    /// <summary>
    /// Sets the basis hit points.
    /// </summary>
    /// <param name="_hp"></param>
    public void SetBasisHP(int _hp)
    {
        if (_hp > Max.HP)
            basis.HP = Max.HP;
        else
            basis.HP = _hp;
    }

    /// <summary>
    /// Sets the basis attack poimts.
    /// </summary>
    /// <param name="_atk"></param>
    public void SetBasisATK(int _atk)
    {
        if (_atk > Max.ATK)
            basis.ATK = Max.ATK;
        else
            basis.ATK = _atk;
    }

    /// <summary>
    /// Sets the buff hit points.
    /// </summary>
    /// <param name="_hp"></param>
    public void SetBuffHP(int _hp)
    {
        if (_hp < 0)
            buff.HP = 0;
        else
            buff.HP = _hp;
    }

    /// <summary>
    /// Sets the buff attack points.
    /// </summary>
    /// <param name="_atk"></param>
    public void SetBuffATK(int _atk)
    {
        if (_atk < 0)
            buff.ATK = 0;
        else
            buff.ATK = _atk;
    }

    /// <summary>
    /// Sets the temporary buff hit points.
    /// </summary>
    /// <param name="_hp"></param>
    public void SetTempBuffHP(int _hp)
    {
        if (_hp < 0)
            temporaryBuff.HP = 0;
        else
            temporaryBuff.HP = _hp;
    }

    /// <summary>
    /// Sets the temporary buff attack points.
    /// </summary>
    /// <param name="_atk"></param>
    public void SetTempBuffATK(int _atk)
    {
        if (_atk < 0)
            temporaryBuff.ATK = 0;
        else
            temporaryBuff.ATK = _atk;
    }
}

