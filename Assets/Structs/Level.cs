public struct Level
    {
    public string Description;
    public int Sell;

    public bool HasAbility;

    public TriggerType TriggerType;
    public int TriggerTimes;
    public int TriggerTimesLimit;
    public DoType DoType;
    public FromWho FromWho;
    public ToWho ToWho;
    public int ToWhoCount;
    public AbilityDuration AbilityDuration;


    // Buff
    public int
        HealthBuff,
        AttackBuff;

    // Summon
    public SoUnit[] SummonUnits;
    public bool SummonForOpponent;
}

