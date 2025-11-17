using UnityEngine;

[CreateAssetMenu(fileName = "TradingCurrency", menuName = "ScriptableObject/TradingCurrency")]
public class SoTradingCurrency : ScriptableObject
{
    public Currency Capacity;

    public Currency RollCost;
    public Currency UnitCost;

    public int LevelAmount;
    public int HealthPortion;

    public Currency[,] Sell;
    public Currency[] RepairCost;
}

