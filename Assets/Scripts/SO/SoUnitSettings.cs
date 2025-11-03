using UnityEngine;

[CreateAssetMenu(fileName = "UnitSettings", menuName = "ScriptableObject/UnitSettings")]
public class SoUnitSettings : ScriptableObject
    {
    public float DraggingScale;
    public float DelayUpdateLevel;
    public float DurationShowDamage;
    public float DurationShowBuff;
    public Vector3 OffsetDragOverOther;
    }
