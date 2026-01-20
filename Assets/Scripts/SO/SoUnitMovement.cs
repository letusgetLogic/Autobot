using UnityEngine;

[CreateAssetMenu(fileName = "UnitMovement", menuName = "ScriptableObject/UnitMovement")]
public class SoUnitMovement : ScriptableObject
    {
    public bool RunBackward = false;
    public float AnimTime = 1f;
    public Vector3 DeltaPosition = new(1f, 0f, 0f);
    public AnimationCurve AnimCurve;
}
