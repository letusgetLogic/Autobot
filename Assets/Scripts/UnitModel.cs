using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnitModel : MonoBehaviour
{
    public SoUnit Data { get; set; }

    public Level CurrentLevel { get; set; }
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

