using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoTradingCurrency))]
class SoTradingCurrencyEditor : Editor
{
    private SoTradingCurrency data;
    private static readonly int NumberWidth = 30;
    private static readonly int ColumnLineWidth = 20;
    private static readonly int ColumnNumberWidth = 40;

    public override void OnInspectorGUI()
    {
        EditorStyles.textField.wordWrap = true;
        data = (SoTradingCurrency)target;
        Draw();
    }

    /// <summary> 
    /// Draw the inspector. 
    /// </summary> 
    /// <exception cref="NotImplementedException"></exception> 
    private void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Capacity per round", GUILayout.Width(120));
        data.Capacity.Coin = EditorGUILayout.IntField(data.Capacity.Coin, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Coins", GUILayout.Width(50));
        data.Capacity.Tool = EditorGUILayout.IntField(data.Capacity.Tool, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Tools", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Roll Cost", GUILayout.Width(120));
        data.RollCost.Coin = EditorGUILayout.IntField(data.RollCost.Coin, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Coins", GUILayout.Width(50));
        data.RollCost.Tool = EditorGUILayout.IntField(data.RollCost.Tool, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Tools", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unit Cost", GUILayout.Width(120));
        data.UnitCost.Coin = EditorGUILayout.IntField(data.UnitCost.Coin, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Coins", GUILayout.Width(50));
        data.UnitCost.Tool = EditorGUILayout.IntField(data.UnitCost.Tool, GUILayout.Width(NumberWidth));
        EditorGUILayout.LabelField("Tools", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Level Amount", GUILayout.Width(120));
        data.LevelAmount = EditorGUILayout.IntField(data.LevelAmount, GUILayout.Width(NumberWidth));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Health Portion", GUILayout.Width(120));
        data.HealthPortion = EditorGUILayout.IntField(data.HealthPortion, GUILayout.Width(NumberWidth));
        EditorGUILayout.EndHorizontal();

        int length = (data.HealthPortion + 1) * data.LevelAmount;
        if (data.Sell == null || 
            data.Sell.Length !=  length)
        {
            data.Sell = new Currency[length];
        }

        if (data.RepairCost == null || data.RepairCost.Length != data.LevelAmount)
            data.RepairCost = new Currency[data.LevelAmount];

        if (data.LevelAmount > 0 && data.HealthPortion > 0)
        {
            DrawSellSheet();
            DrawRepairCost();
        }
    }

    private void DrawSellSheet()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sell", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(100));

        for (int j = 0; j < data.LevelAmount; j++)
        {
            EditorGUILayout.LabelField("|", GUILayout.Width(ColumnLineWidth));
            EditorGUILayout.LabelField($"      LV {j + 1}", GUILayout.Width(83));
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < data.HealthPortion + 1; i++)
            DrawRow(i);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(100));

        for (int j = 0; j < data.LevelAmount; j++)
        {
            EditorGUILayout.LabelField("|", GUILayout.Width(ColumnLineWidth));
            EditorGUILayout.LabelField($"Coins", GUILayout.Width(ColumnNumberWidth));
            EditorGUILayout.LabelField($"Tools", GUILayout.Width(ColumnNumberWidth));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void DrawRow(int i)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Durability {i}", GUILayout.Width(100));

        for (int j = 0; j < data.LevelAmount; j++)
        {
            EditorGUILayout.LabelField("|", GUILayout.Width(ColumnLineWidth));
            int index = SoTradingCurrency.ConvertToIndex1D(data.HealthPortion, i, j);
            data.Sell[index].Coin = EditorGUILayout.IntField(data.Sell[index].Coin, GUILayout.Width(ColumnNumberWidth));
            data.Sell[index].Tool = EditorGUILayout.IntField(data.Sell[index].Tool, GUILayout.Width(ColumnNumberWidth));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawRepairCost()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Repair per");
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"durability unit", GUILayout.Width(100));

        for (int i = 0; i < data.RepairCost.Length; i++)
        {
            EditorGUILayout.LabelField("|", GUILayout.Width(ColumnLineWidth));
            data.RepairCost[i].Coin = EditorGUILayout.IntField(data.RepairCost[i].Coin, GUILayout.Width(ColumnNumberWidth));
            data.RepairCost[i].Tool = EditorGUILayout.IntField(data.RepairCost[i].Tool, GUILayout.Width(ColumnNumberWidth));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }
}