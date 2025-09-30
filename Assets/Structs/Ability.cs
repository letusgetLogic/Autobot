public struct Ability
    {
    public string
       Description;

    public bool HasAbility;

    public TriggerType TriggerType;
    public DoType DoType;
    public FromWho FromWho;
    public ToWho ToWho;
    public AbilityDuration AbilityDuration;

    public int
        Cost,
        ToWhoCount,
        TriggerTimes,
        TriggerTimesLimit;

    // Buff
    public int
        HealthBuff,
        AttackBuff;

    // Summon
}

