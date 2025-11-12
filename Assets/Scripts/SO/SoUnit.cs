using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit : ScriptableObject
{
    public Sprite Sprite;

    public int ID;
    public string Name;

    public int Health;
    public int Attack;
    public int Energy;
    public SoIntVariable Cost;

    public int LevelLimit;
    public Level[] Levels ;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

