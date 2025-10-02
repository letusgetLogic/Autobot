using log4net.Core;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoUnit))]
class SoUnitEditor : Editor
{
    private SoUnit data;
    private Level[] levels;
    private bool[]
        showLevelSections,
        showTriggerSections;

    public override void OnInspectorGUI()
    {
        EditorStyles.textField.wordWrap = true;
        data = (SoUnit)target;
        levels = ((SoUnit)target).Levels;
        Draw();
    }

    /// <summary> 
    /// Draw the inspector. 
    /// </summary> 
    /// <exception cref="NotImplementedException"></exception> 
    private void Draw()
    {
        data.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", data.Sprite, typeof(Sprite), false);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID", GUILayout.Width(50));
        data.ID = EditorGUILayout.TextField(data.ID);
        EditorGUILayout.LabelField("Name", GUILayout.Width(50));
        data.Name = EditorGUILayout.TextField(data.Name);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attack", GUILayout.Width(50));
        data.Attack = EditorGUILayout.IntField(data.Attack);
        EditorGUILayout.LabelField("Health", GUILayout.Width(50));
        data.Health = EditorGUILayout.IntField(data.Health);
        EditorGUILayout.EndHorizontal();

        data.Cost = EditorGUILayout.IntField("Cost", data.Cost);
        data.LevelLimit = EditorGUILayout.IntField("Level Limit", data.LevelLimit);


        if (data.LevelLimit <= 0)
            return;
        else
            if (data.Levels == null || data.Levels.Length != data.LevelLimit)
        {
            data.Levels = new Level[data.LevelLimit];
            showLevelSections = new bool[data.LevelLimit];
            showTriggerSections = new bool[data.LevelLimit];
        }

        if (GUILayout.Button("Synchronize Levels"))
        {
            for (int i = 1; i < data.LevelLimit; i++)
            {
                data.Levels[i] = data.Levels[0];
                data.Levels[i].Sell = data.Levels[0].Sell + 1;
            }
        }

        EditorGUI.indentLevel++;

        for (int i = 0; i < data.LevelLimit; i++)
        {
            DrawLevel(i);
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Draw the data for a specific level. 
    /// </summary>
    /// <param name="_i"></param>
    private void DrawLevel(int _i)
    {
        showLevelSections[_i] = EditorGUILayout.Foldout(showLevelSections[_i], $"Level {_i + 1}");
        if (!showLevelSections[_i])
            return;

        EditorGUI.indentLevel++;

        levels[_i].Description = EditorGUILayout.TextField("Description", levels[_i].Description);
        levels[_i].Sell = EditorGUILayout.IntField("Sell", levels[_i].Sell);
        levels[_i].HasAbility = EditorGUILayout.Toggle("Has Ability", levels[_i].HasAbility);

        if (levels[_i].HasAbility)
        {
            levels[_i].TriggerType = (TriggerType)EditorGUILayout.EnumPopup("Trigger", levels[_i].TriggerType);
                DrawTriggerTimes(ref levels[_i], _i);

            DrawDoType(ref levels[_i]);

            levels[_i].AbilityDuration = (AbilityDuration)EditorGUILayout.EnumPopup("Duration", levels[_i].AbilityDuration);
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Draws the trigger times and their limits.
    /// </summary>
    private void DrawTriggerTimes(ref Level _level, int _i)
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();

        showTriggerSections[_i] = EditorGUILayout.Toggle(showTriggerSections[_i]);

        if (showTriggerSections[_i])
        {
            EditorGUILayout.LabelField("- Times", GUILayout.Width(80));
            _level.TriggerTimes = EditorGUILayout.IntField(_level.TriggerTimes, GUILayout.Width(80));

            EditorGUILayout.LabelField("- Limit", GUILayout.Width(80));
            _level.TriggerTimesLimit = EditorGUILayout.IntField(_level.TriggerTimesLimit, GUILayout.Width(80));
        }

        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;

    }

    /// <summary> 
    /// Draw the do type and its attributes. 
    /// </summary> 
    /// <param name="_level"></param> 
    private void DrawDoType(ref Level _level)
    {
        _level.DoType = (DoType)EditorGUILayout.EnumPopup("Do", _level.DoType);
        DrawFromWhoToWho(ref _level);

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
    /// Draws the "From Who" and "To Who" fields.
    /// </summary>
    /// <param name="_index"></param>
    private void DrawFromWhoToWho(ref Level _level)
    {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("From", GUILayout.Width(80));
        _level.FromWho = (FromWho)EditorGUILayout.EnumPopup(_level.FromWho);

        EditorGUILayout.LabelField("To", GUILayout.Width(80));
        _level.ToWhoCount = EditorGUILayout.IntField(_level.ToWhoCount, GUILayout.Width(80));
        _level.ToWho = (ToWho)EditorGUILayout.EnumPopup(_level.ToWho);

        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    /// <summary> 
    /// Draw the buff attributes. 
    /// </summary> /// <param name="_level"></param> 
    private void DrawBuffAttributes(ref Level _level)
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("+ AP", GUILayout.Width(80));
        _level.AttackBuff = EditorGUILayout.IntField(_level.AttackBuff, GUILayout.Width(80));
        EditorGUILayout.LabelField("+ HP", GUILayout.Width(80));
        _level.HealthBuff = EditorGUILayout.IntField(_level.HealthBuff, GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    /// <summary> 
    /// Draw the summon attribute 
    /// </summary> /
    /// <param name="_level"></param> 
    private void DrawSummonAttributes(ref Level _level)
    {
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        _level.SummonUnits = new SoUnit[EditorGUILayout.IntField(_level.SummonUnits.Length, GUILayout.Width(80))];
        EditorGUILayout.LabelField("Summoned Units");
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < _level.SummonUnits.Length; i++)
        {
            _level.SummonUnits[i] = (SoUnit)EditorGUILayout.ObjectField($"Unit {i + 1}",
                _level.SummonUnits[i], typeof(SoUnit), false);
        }
        _level.SummonForOpponent = EditorGUILayout.Toggle("Summon for Opponent", _level.SummonForOpponent);

        EditorGUI.indentLevel--;
    }
}