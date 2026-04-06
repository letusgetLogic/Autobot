using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSettings", menuName = "ScriptableObject/TutorialSettings")]
public class SoTutorialSettings : ScriptableObject
{
    [Range(0f, 2f)] public float Delay;
    public float DelayAFK;

    public bool AutoCompleted = false;
    public List<InputKey> AllowedInputs;
}
