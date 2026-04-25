using System;

/// <summary>
/// Data structure to save unit data between phases and sessions.
/// </summary>
[Serializable]
public class SaveUnitData
{
    public string _ID { get; set; }
    public int Index { get; set; }
    public UnitType UnitType { get; set; }

    public Attribute Max;
    public int MaxXP { get; set; }

    /// <summary>
    /// The current attributes, which are displayed.
    /// </summary>
    public Attribute Cur => current;
    private Attribute current;

    /// <summary>
    /// The basis attributres, which scale up from merging.
    /// </summary>
    public Attribute Basis => basis;
    private Attribute basis;

    /// <summary>
    /// The buff attributes, which scale up from abilities only during shop phase, are pernament.
    /// </summary>
    public Attribute Buff => buff;
    private Attribute buff;

    /// <summary>
    /// The temporary buff attributes, which scale up from abilities, 
    /// was setted to 0 at the end of battle phase.
    /// </summary>
    public Attribute TempBuff => temporaryBuff;
    private Attribute temporaryBuff;

    public int FullHP => Basis.HP + Buff.HP + TempBuff.HP;
    public int FullATK => Basis.ATK + Buff.ATK + TempBuff.ATK;

    public int XP => xp;
    private int xp;

    public int Durability { get; set; }
    public float DurabilityRatio { get; set; }

    public UnitState UnitState { get; set; }

    public bool IsTeamLeft { get; set; }

    
    /// <summary>
    /// Default constructor
    /// </summary>
    public SaveUnitData()
    {
    }
    
    /// <summary>
    /// Constructor for updating level without model
    /// </summary>
    public SaveUnitData(int _xp, Attribute _cur, Attribute _basis, Attribute _buff, Attribute _tempBuff)
    {
        xp = _xp;
        current = _cur;
        basis = _basis;
        buff = _buff;
        temporaryBuff = _tempBuff;
    }

    /// <summary>
    /// Makes a copy of the reference.
    /// </summary>
    /// <param name="_other"></param>
    public SaveUnitData(SaveUnitData _other)
    {
        _ID = _other._ID;
        Index = _other.Index;
        UnitType = _other.UnitType;
        Max = _other.Max;
        MaxXP = _other.MaxXP;
        current = _other.current;
        basis = _other.basis;
        buff = _other.buff;
        temporaryBuff = _other.temporaryBuff;
        xp = _other.xp;
        Durability = _other.Durability;
        DurabilityRatio = _other.DurabilityRatio;
        UnitState = _other.UnitState;
        IsTeamLeft = _other.IsTeamLeft;
    }


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
    /// <param name="_updateView"></param>
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

