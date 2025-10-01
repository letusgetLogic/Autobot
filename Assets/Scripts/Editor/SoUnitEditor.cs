using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoUnit))]
class SoUnitEditor : Editor
{
    private SerializedProperty spriteProp;
    private SerializedProperty nameProp;
    private SerializedProperty healthProp;
    private SerializedProperty attackProp;
    private SerializedProperty costProp;
    private SerializedProperty levelLimitProp;
    private SerializedProperty levelsProp;

    private void OnEnable()
    {
        spriteProp = serializedObject.FindProperty("Sprite");
        nameProp = serializedObject.FindProperty("Name");
        healthProp = serializedObject.FindProperty("Health");
        attackProp = serializedObject.FindProperty("Attack");
        costProp = serializedObject.FindProperty("Cost");
        levelLimitProp = serializedObject.FindProperty("LevelLimit");
        levelsProp = serializedObject.FindProperty("Levels");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Basiswerte ---
        EditorGUILayout.PropertyField(spriteProp);
        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(healthProp);
        EditorGUILayout.PropertyField(attackProp);
        EditorGUILayout.PropertyField(costProp);

        // --- Level Limit ---
        EditorGUILayout.PropertyField(levelLimitProp);

        // Array nur dann anpassen, wenn Wert unterschiedlich ist
        int wantedSize = Mathf.Max(0, levelLimitProp.intValue);
        if (levelsProp.arraySize != wantedSize)
        {
            levelsProp.arraySize = wantedSize;
        }

        // --- Levels zeichnen ---
        for (int i = 0; i < levelsProp.arraySize; i++)
        {
            SerializedProperty levelProp = levelsProp.GetArrayElementAtIndex(i);
            DrawLevel(i + 1, levelProp);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLevel(int index, SerializedProperty levelProp)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Lv. {index}", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("Description"));
        EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("Sell"));
        EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("HasAbility"));

        if (levelProp.FindPropertyRelative("HasAbility").boolValue)
        {
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("TriggerType"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("TriggerTimes"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("TriggerTimesLimit"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("DoType"));

            var doType = (DoType)levelProp.FindPropertyRelative("DoType").enumValueIndex;
            if (doType == DoType.Buff)
            {
                EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("HealthBuff"));
                EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("AttackBuff"));
            }
            else if (doType == DoType.Summon)
            {
                EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("SummonUnits"), true);
                EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("SummonForOpponent"));
            }

            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("FromWho"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("ToWho"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("ToWhoCount"));
            EditorGUILayout.PropertyField(levelProp.FindPropertyRelative("AbilityDuration"));
        }
    }
}
