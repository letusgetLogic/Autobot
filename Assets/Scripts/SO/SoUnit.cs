using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; set; }

    [field: SerializeField] public int ID { get; set; }
    [field: SerializeField] public string Name { get; set; }

    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public int Attack { get; set; }
    [field: SerializeField] public SoIntVariable Cost { get; set; }

    [field: SerializeField] public int LevelLimit { get; set; }
    [field: SerializeField] public Level[] Levels { get; set; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

