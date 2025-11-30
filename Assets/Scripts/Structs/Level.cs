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

    // Buff
    public int HealthBuff;
    public int AttackBuff;

    // Summon
    public int UnitLimit;
    public SoUnit[] SummonUnits;
    public bool SummonForOpponent;

    // Deal
    public int DealDamage;

    // Gain
    public int GainCoin;

}

