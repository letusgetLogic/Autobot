using UnityEngine;

[CreateAssetMenu(fileName = "UnitSettings", menuName = "ScriptableObject/UnitSettings")]
public class SoUnitSettings : ScriptableObject
    {
    public float DraggingScale;
    public float DelayUpdateLevel;
    public float DurationShowDamage;
    public float DurationShowTemporaryValue;
    public Vector3 OffsetDragOverOther;
    public Vector3 OffsetMoveOverOther;
    }
