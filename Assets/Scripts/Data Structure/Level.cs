[System.Serializable]
public struct Level
{
    public int Index;

    public string Description;

    public bool HasAbility;

    public SoIntVariable ConsumedEnergy;
    public TriggerType TriggerType;
    public int TriggerTimes;
    public int TriggerTimesLimit;
    public DoType DoType;
    public FromWho FromWho;
    public ToWho ToWho;
    public int ToWhoCount;
    public AbilityDuration AbilityDuration;

    // Manage Attributes
    public Attribute Buff;
    public Attribute Debuff;

    // Summon
    public int UnitLimit;
    public SoUnit[] SummonUnits;
    public bool SummonForOpponent;

    // Gain
    public int GainCoin;

}

