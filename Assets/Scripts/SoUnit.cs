using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit :ScriptableObject
{
    public Sprite Sprite;

    public string
        Name;

    public int 
        Health,
        Attack;

    public Ability AbilityLv1;
    public Ability AbilityLv2;
    public Ability AbilityLv3;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

