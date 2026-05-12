using UnityEngine;

[CreateAssetMenu(fileName = "LerpMovement", menuName = "ScriptableObject/LerpMovement")]
public class SoLerpMovementSettings : ScriptableObject
    {
    public bool RunBackward = false;
    public float AnimTime = 1f;
    public Vector3 DeltaPosition = new(1f, 0f, 0f);
    public AnimationCurve AnimCurve;
}
