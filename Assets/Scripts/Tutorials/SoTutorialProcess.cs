using UnityEngine;

[CreateAssetMenu(fileName = "TutorialProcess", menuName = "ScriptableObject/TutorialProcess")]
public class SoTutorialProcess : ScriptableObject
{
    [Range(0f, 2f)] public float[] Delays;
}
