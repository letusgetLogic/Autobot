using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TradingCurrency", menuName = "ScriptableObject/TradingCurrency")]
public class SoTradingCurrency : ScriptableObject
{
    public Currency Capacity;

    public Currency RollCost;
    public Currency UnitCost;

    public int LevelAmount;
    public int HealthPortion;

    public Currency[] Sell;
    public Currency[] RepairCost;

    public int SellIndexLength => (HealthPortion + 1) * LevelAmount;
    public int MaxSellIndex => SellIndexLength - 1;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>
    /// Return _durability * _levelAmount + _levelIndex.
    /// </summary>
    /// <param name="_durability"></param>
    /// <param name="_levelAmount"></param>
    /// <param name="_levelIndex"></param>
    /// <param name="_calculateWithRepairSystem"></param>
    /// <returns></returns>
    public static int ConvertToIndex1D(int _durability, int _levelAmount, int _levelIndex, bool _calculateWithRepairSystem)
    {
        if (_calculateWithRepairSystem == false)
        {
            _durability = 1;
        }
        return _durability * _levelAmount + _levelIndex;
    }
}

