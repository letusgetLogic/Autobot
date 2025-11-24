using System;

[Serializable]
public struct SaveUnitData
{
    public bool HasReference { get; set; }
    public string ID { get; set; }
    public int Index { get; set; }

    // varies during battle.
    public readonly Attribute Cur => current; 
    private Attribute current;

    // varies during merging.
    public readonly Attribute Basis => basis; 
    private Attribute basis;

    // varies only during shop phase, it is pernament.
    public readonly Attribute Buff => buff;
    private Attribute buff;

    // on the end of battle, this was setted to 0.
    public readonly Attribute TempBuff => temporaryBuff;
    private Attribute temporaryBuff;

    public readonly int FullHP => Basis.HP + Buff.HP + TempBuff.HP;
    public readonly int FullATK => Basis.ATK + Buff.ATK + TempBuff.ATK;

    public int Durability { get; set; }

    public readonly int XP => xp;
    private int xp;
    public UnitState UnitState { get; set; }

    public bool IsTeam1 { get; set; }
    public float DurabilityRatio { get; set; }

    public void SetHP(int _hp)
    {
        int max = PackManager.Instance.MyPack.MaxHP.Value;
        if (_hp > max)
            current.HP = max;
        else
            current.HP = _hp;
    }

    public void SetATK(int _atk)
    {
        int max = PackManager.Instance.MyPack.MaxATK.Value;
        if (_atk > max)
            current.ATK = max;
        else
            current.ATK = _atk;
    }

    public void SetEnergy(int _energy)
    {
        int max = PackManager.Instance.MyPack.MaxEnergy.Value;
        if (_energy > max)
            current.Energy = max;
        else
            current.Energy = _energy;
    }

    public void SetXP(int _xp)
    {
        int max = PackManager.Instance.MyPack.XpToLv3.Value;
        if (_xp > max)
            xp = max;
        else
            xp = _xp;
    }

    public void SetBasisHP(int _hp)
    {
        int max = PackManager.Instance.MyPack.MaxHP.Value;
        if (_hp > max)
            basis.HP = max;
        else
            basis.HP = _hp;
    }

    public void SetBasisATK(int _atk)
    {
        int max = PackManager.Instance.MyPack.MaxATK.Value;
        if (_atk > max)
            basis.ATK = max;
        else
            basis.ATK = _atk;
    }

    public void SetBuffHP(int _hp)
    {
        if (_hp < 0)
            buff.HP = 0;
        else
            buff.HP = _hp;
    }

    public void SetBuffATK(int _atk)
    {
        if (_atk < 0)
            buff.ATK = 0;
        else
            buff.ATK = _atk;
    }

    public void SetTempBuffHP(int _hp)
    {
        if (_hp < 0)
            temporaryBuff.HP = 0;
        else
            temporaryBuff.HP = _hp;
    }

    public void SetTempBuffATK(int _atk)
    {
        if (_atk < 0)
            temporaryBuff.ATK = 0;
        else
            temporaryBuff.ATK = _atk;
    }

}

