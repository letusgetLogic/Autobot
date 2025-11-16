using UnityEngine;

[CreateAssetMenu(fileName = "TradingCurrency", menuName = "ScriptableObject/TradingCurrency", order = 1)]
public class SoTradingCurrency : ScriptableObject
{
    public int Capacity;

    public Currency RollCost;
    public Currency UnitCost;

    public Currency[,] Sell;

    // Repair System
    public int HealthPortion;
    public Currency RepairCostLv1;
    public Currency RepairCostLv2;
    public Currency RepairCostLv3;

    // Charging Energy System
    public int ConsumedEnergy;
    public int ChargingEnergy;

}

