using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit : ScriptableObject
{
    public Sprite Sprite { get; set; }

    public int ID { get; set; }
    public string Name { get; set; }

    public int Health { get; set; }
    public int Attack { get; set; }
    public SoIntVariable Cost { get; set; }

    public int LevelLimit { get; set; }
    public Level[] Levels { get; set; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

