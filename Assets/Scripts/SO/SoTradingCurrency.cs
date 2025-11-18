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

    public Currency[] Sell { get; set; }
    public Currency[] RepairCost;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

