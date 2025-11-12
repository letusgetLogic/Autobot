using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoUnit))]
class SoUnitEditor : Editor
{
    private SoUnit data;

    private bool[]
        showLevelSections,
        showTriggerSections;

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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID", GUILayout.Width(50));
        data.ID = EditorGUILayout.IntField(data.ID);
        EditorGUILayout.LabelField("Name", GUILayout.Width(50));
        data.Name = EditorGUILayout.TextField(data.Name);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attack", GUILayout.Width(50));
        data.Attack = EditorGUILayout.IntField(data.Attack);
        EditorGUILayout.LabelField("Health", GUILayout.Width(50));
        data.Health = EditorGUILayout.IntField(data.Health);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Energy", GUILayout.Width(50));
        data.Energy = EditorGUILayout.IntField(data.Energy);
        EditorGUILayout.EndHorizontal();

        data.Cost = (SoIntVariable)EditorGUILayout.ObjectField("Cost", data.Cost, typeof(SoIntVariable), false);
        data.LevelLimit = EditorGUILayout.IntField("Level Limit", data.LevelLimit);

        if (data.Levels == null || data.Levels.Length != data.LevelLimit)
            data.Levels = new Level[data.LevelLimit];

        if (showLevelSections == null || showLevelSections.Length != data.Levels.Length)
        {
            showLevelSections = new bool[data.Levels.Length];
            for (int i = 0; i < showLevelSections.Length; i++)
            {
                showLevelSections[i] = true;
            }
        }
        if (showTriggerSections == null || showTriggerSections.Length != data.Levels.Length)
            showTriggerSections = new bool[data.Levels.Length];

        if (GUILayout.Button("Synchronize Levels"))
        {
            for (int i = 1; i < data.LevelLimit; i++)
            {
                data.Levels[i] = data.Levels[0];
                data.Levels[i].Sell = data.Levels[i - 1].Sell + 1;
                data.Levels[i].SummonUnits = null;
            }
        }

        EditorGUI.indentLevel++;

        for (int i = 0; i < data.LevelLimit; i++)
        {
            data.Levels[i].Number = i + 1;
            DrawLevel(ref data.Levels[i], i);
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Draw the data for a specific level. 
    /// </summary>
    /// <param name="_i"></param>
    private void DrawLevel(ref Level _level, int _i)
    {
        showLevelSections[_i] = EditorGUILayout.Foldout(showLevelSections[_i], $"Level {_i + 1}");
        if (!showLevelSections[_i])
            return;

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Description", GUILayout.Width(100));
        _level.Description = EditorGUILayout.TextArea(_level.Description);

        _level.Sell = EditorGUILayout.IntField("Sell", _level.Sell);
        _level.HasAbility = EditorGUILayout.Toggle("Has Ability", _level.HasAbility);

        if (_level.HasAbility)
        {
            _level.TriggerType = (TriggerType)EditorGUILayout.EnumPopup("Trigger", _level.TriggerType);
            DrawTriggerTimes(ref _level, _i);

            DrawDoType(ref _level);

            _level.AbilityDuration = (AbilityDuration)EditorGUILayout.EnumPopup("Duration", _level.AbilityDuration);
        }
        else
        {
            // Reset ability-related fields if HasAbility is false
            _level.TriggerType = TriggerType.None;
            _level.TriggerTimes = 0;
            _level.TriggerTimesLimit = 0;
            _level.DoType = DoType.None;
            _level.FromWho = FromWho.None;
            _level.ToWho = ToWho.None;
            _level.ToWhoCount = 0;
            _level.AbilityDuration = AbilityDuration.None;
            // Buff attributes
            _level.HealthBuff = 0;
            _level.AttackBuff = 0;
            // Summon attributes
            _level.UnitLimit = 0;
            _level.SummonUnits = null;
            _level.SummonForOpponent = false;
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

        EditorGUILayout.LabelField("> Times", GUILayout.Width(100));
        showTriggerSections[_i] = EditorGUILayout.Toggle(showTriggerSections[_i]);

        if (showTriggerSections[_i])
        {
            EditorGUILayout.LabelField("- Times", GUILayout.Width(90));
            _level.TriggerTimes = EditorGUILayout.IntField(_level.TriggerTimes, GUILayout.Width(80));

            EditorGUILayout.LabelField("- Limit", GUILayout.Width(90));
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

        switch (_level.DoType)
        {
            case DoType.Buff:
                DrawBuffAttributes(ref _level);
                break;
            case DoType.Summon:
                DrawSummonAttributes(ref _level);
                break;
            case DoType.Deal:
                DrawDealAttributes(ref _level);
                break;
            case DoType.GainCoin:
                DrawGainCoinAttributes(ref _level);
                break;
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
        _level.UnitLimit = EditorGUILayout.IntField(_level.UnitLimit, GUILayout.Width(80));
        EditorGUILayout.LabelField("Summoned Units");
        EditorGUILayout.EndHorizontal();

        if (_level.SummonUnits == null || _level.SummonUnits.Length != _level.UnitLimit)
        {
            _level.SummonUnits = new SoUnit[_level.UnitLimit];
        }

        if (_level.SummonUnits == null)
            return;

        for (int i = 0; i < _level.SummonUnits.Length; i++)
        {
            _level.SummonUnits[i] = (SoUnit)EditorGUILayout.ObjectField($"Unit {i + 1}",
                _level.SummonUnits[i], typeof(SoUnit), false);
        }
        _level.SummonForOpponent = EditorGUILayout.Toggle("Summon for Opponent", _level.SummonForOpponent);

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Draw the deal attributes.
    /// </summary>
    /// <param name="_level"></param>
    private void DrawDealAttributes(ref Level _level)
    {
        EditorGUI.indentLevel++;
        _level.DealDamage = EditorGUILayout.IntField("Deal Damage", _level.DealDamage);
        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Draw the gain coin attributes.
    /// </summary>
    /// <param name="_level"></param>
    private void DrawGainCoinAttributes(ref Level _level)
    {
        EditorGUI.indentLevel++;
        _level.GainCoin = EditorGUILayout.IntField("Gain Coin", _level.GainCoin);
        EditorGUI.indentLevel--;
    }
}