using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoUnit))]
class SoUnitEditor : Editor
{
    private SoUnit data;
    public override void OnInspectorGUI()
    {
        EditorStyles.textField.wordWrap = true;
        data = (SoUnit)target;
        Draw();
    }

    /// <summary> 
    /// Draw the inspector. 
    /// </summary> 
    /// <exception cref="NotImplementedException"></exception> 
    private void Draw()
    {
        data.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", data.Sprite, typeof(Sprite), false);
        data.Name = EditorGUILayout.TextField("Name", data.Name);
        data.Health = EditorGUILayout.IntField("Health", data.Health);
        data.Attack = EditorGUILayout.IntField("Attack", data.Attack);
        data.Cost = EditorGUILayout.IntField("Cost", data.Cost);
        data.LevelLimit = EditorGUILayout.IntField("Level Limit", data.LevelLimit);

        if (data.LevelLimit <= 0)
            return;
        else
            if (data.Levels == null || data.Levels.Length != data.LevelLimit)
                 data.Levels = new Level[data.LevelLimit];

        for (int i = 0; i < data.LevelLimit; i++)
        {
            DrawLevel(i + 1, ref data.Levels[i]);
        }
    }

    /// <summary> 
    /// Draw the data for a specific level. 
    /// </summary> 
    /// <param name="amount"></param> 
    /// <param name="_level"></param> 
    private void DrawLevel(int amount, ref Level _level)
    {
        EditorGUILayout.LabelField($"Lv. {amount}");
        _level.Description = EditorGUILayout.TextField("Description", _level.Description);

        _level.Sell = EditorGUILayout.IntField("Sell", _level.Sell);
        _level.HasAbility = EditorGUILayout.Toggle("Has Ability", _level.HasAbility);
        if (_level.HasAbility)
        {
            _level.TriggerType = (TriggerType)EditorGUILayout.EnumPopup("Trigger Type", _level.TriggerType);
            _level.TriggerTimes = EditorGUILayout.IntField("Trigger Times", _level.TriggerTimes);
            _level.TriggerTimesLimit = EditorGUILayout.IntField("Trigger Times Limit", _level.TriggerTimesLimit);

            DrawDoType(ref _level);

            _level.FromWho = (FromWho)EditorGUILayout.EnumPopup("From", _level.FromWho);
            _level.ToWho = (ToWho)EditorGUILayout.EnumPopup("To", _level.ToWho);
            _level.ToWhoCount = EditorGUILayout.IntField("Amount", _level.ToWhoCount);
            _level.AbilityDuration = (AbilityDuration)EditorGUILayout.EnumPopup("Duration", _level.AbilityDuration);
        }
    }

    /// <summary> 
    /// Draw the do type and its attributes. 
    /// </summary> 
    /// <param name="_level"></param> 
    private void DrawDoType(ref Level _level)
    {
        _level.DoType = (DoType)EditorGUILayout.EnumPopup("Ability Type", _level.DoType);
        if (_level.DoType == DoType.Buff)
        {
            DrawBuffAttributes(ref _level);
        }
        else if (_level.DoType == DoType.Summon)
        {
            DrawSummonAttributes(ref _level);
        }
    }

    /// <summary> 
    /// Draw the buff attributes. 
    /// </summary> /// <param name="_level"></param> 
    private void DrawBuffAttributes(ref Level _level)
    {
        _level.HealthBuff = EditorGUILayout.IntField("Buff Health", _level.HealthBuff);
        _level.AttackBuff = EditorGUILayout.IntField("Buff Attack", _level.AttackBuff);
    }

    /// <summary> 
    /// Draw the summon attribute 
    /// </summary> /
    /// <param name="_level"></param> 
    private void DrawSummonAttributes(ref Level _level)
    {
        EditorGUILayout.IntField("Amount of Summoned Units", _level.SummonUnits.Length);
        for (int i = 0; i < _level.SummonUnits.Length; i++)
        {
            _level.SummonUnits[i] = (SoUnit)EditorGUILayout.ObjectField($"Unit {i + 1}",
                _level.SummonUnits[i], typeof(SoUnit), false);
        }
        _level.SummonForOpponent = EditorGUILayout.Toggle("Summon for Opponent", _level.SummonForOpponent);
    }
}