using System;

[Serializable]
public struct SaveUnitData
{
    public bool HasReference { get; set; }
    public int Index { get; set; }

    public int BasisHp { get; set; }
    public int BasisAtk { get; set; }
    
    public int BuffHp { get => buffHp; set { buffHp = NegativToZero(value); } }
    private int buffHp;
    public int BuffAtk { get => buffAtk; set { buffAtk = NegativToZero(value); } }
    private int buffAtk;

    public int BuffTempHp { get => buffTempHp; set { buffTempHp = NegativToZero(value); } }
    private int buffTempHp;
    public int BuffTempAtk { get => buffTempAtk; set { buffTempAtk = NegativToZero(value); } }
    private int buffTempAtk;

    public int Hp { get; private set; }
    public int Atk { get; set; }

    public int Durability { get; set; }
    public int Energy { get; set; }
    public int XP
    {
        get { return xp; }
        set
        {
            if (value > PackManager.Instance.MyPack.XpToLv3.Value)
                xp = PackManager.Instance.MyPack.XpToLv3.Value;
            else xp = value;
        }
    }
    private int xp;
    public UnitState UnitState { get; set; }

    public bool IsTeam1 { get; set; }
    public float DurabilityRatio { get; set; }

    /// <summary>
    /// Converts a negative integer to zero.
    /// </summary>
    /// <param name="value">The integer value to be evaluated.</param>
    /// <returns>The original value if it is zero or positive; otherwise, zero.</returns>
    private int NegativToZero(int value)
    {
        if (value < 0)
            return 0;
        return value;
    }

    public void SetHp(int hp)
    {
        Hp = hp;
    }
}

