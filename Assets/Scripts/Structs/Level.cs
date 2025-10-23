using UnityEngine;

public struct Level
    {
    [field: SerializeField] public int Number { get; set; }
   
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public int Sell { get; set; }
  
    [field: SerializeField] public bool HasAbility { get; set; }
 
    [field: SerializeField] public TriggerType TriggerType { get; set; }
    [field: SerializeField] public int TriggerTimes { get; set; }
    [field: SerializeField] public int TriggerTimesLimit { get; set; }
    [field: SerializeField] public DoType DoType { get; set; }
    [field: SerializeField] public FromWho FromWho { get; set; }
    [field: SerializeField] public ToWho ToWho { get; set; }
    [field: SerializeField] public int ToWhoCount { get; set; }
    [field: SerializeField] public AbilityDuration AbilityDuration { get; set; }
  
     // Buff
    [field: SerializeField] public int HealthBuff { get; set; }
    [field: SerializeField] public int AttackBuff { get; set; }
   
     // Summon
    [field: SerializeField] public int UnitLimit { get; set; }
    [field: SerializeField] public SoUnit[] SummonUnits { get; set; }
    [field: SerializeField] public bool SummonForOpponent { get; set; }
  
     // Deal
    [field: SerializeField] public int DealDamage { get; set; }
  
     // Gain
    [field: SerializeField] public int GainCoin { get; set; }
}

