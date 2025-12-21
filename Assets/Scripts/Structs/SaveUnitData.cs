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

    public readonly int FullHP => Basis.HP + Buff.HP + TempBuff.HP;
    public readonly int FullATK => Basis.ATK + Buff.ATK + TempBuff.ATK;

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
        int max = PackManager.Instance.MyPack.MaxHP.Value;
        if (_hp > max)
            current.HP = max;
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
        int max = PackManager.Instance.MyPack.MaxATK.Value;

        if (_atk < 0)
        {
            current.ATK = 0;
            return;
        }
        if (_atk > max)
            current.ATK = max;
        else
            current.ATK = _atk;
    }

    /// <summary>
    /// Sets the current energy.
    /// </summary>
    /// <param name="_energy"></param>
    public void SetEnergy(int _energy)
    {
        int max = PackManager.Instance.MyPack.MaxEnergy.Value;

        if (_energy < 0)
        {
            current.ENG = 0;
            return;
        }
        if (_energy > max)
            current.ENG = max;
        else
            current.ENG = _energy;
    }

    /// <summary>
    /// Sets experience points.
    /// </summary>
    /// <param name="_xp"></param>
    public void SetXP(int _xp)
    {
        int max = PackManager.Instance.MyPack.XpToLv3.Value;
        if (_xp > max)
            xp = max;
        else
            xp = _xp;
    }

    /// <summary>
    /// Sets the basis hit points.
    /// </summary>
    /// <param name="_hp"></param>
    public void SetBasisHP(int _hp)
    {
        int max = PackManager.Instance.MyPack.MaxHP.Value;
        if (_hp > max)
            basis.HP = max;
        else
            basis.HP = _hp;
    }

    /// <summary>
    /// Sets the basis attack poimts.
    /// </summary>
    /// <param name="_atk"></param>
    public void SetBasisATK(int _atk)
    {
        int max = PackManager.Instance.MyPack.MaxATK.Value;
        if (_atk > max)
            basis.ATK = max;
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

    /// <summary>
    /// Return boolean, if it is a robot.
    /// </summary>
    /// <returns></returns>
    public bool IsRobot()
    {
        if (UnitType == UnitType.Robot || UnitType == UnitType.SummonedRobot)
            return true;

        return false;
    }
}

