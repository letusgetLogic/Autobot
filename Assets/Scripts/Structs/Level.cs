public struct Level
    {
    public int Number { get; set; }

    public string Description { get; set; }
    public int Sell { get; set; }

    public bool HasAbility { get; set; }

    public TriggerType TriggerType { get; set; }
    public int TriggerTimes { get; set; }
    public int TriggerTimesLimit { get; set; }
    public DoType DoType { get; set; }
    public FromWho FromWho { get; set; }
    public ToWho ToWho { get; set; }
    public int ToWhoCount { get; set; }
    public AbilityDuration AbilityDuration { get; set; }

    // Buff
    public int HealthBuff { get; set; }
    public int AttackBuff { get; set; }

    // Summon
    public int UnitLimit { get; set; }
    public SoUnit[] SummonUnits { get; set; }
    public bool SummonForOpponent { get; set; }

    // Deal
    public int DealDamage { get; set; }

    // Gain
    public int GainCoin { get; set; }
}

