using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit : ScriptableObject
{
    public Sprite Sprite;

    public string 
        ID,
        Name;

    public int
        Health,
        Attack,
        Cost;

    public int LevelLimit;
    public Level[] Levels;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

