using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSettings", menuName = "ScriptableObject/TutorialSettings")]
public class SoTutorialSettings : ScriptableObject
{
    public float Delay;
    public float Duration;

    public bool AutoCompleted = false;
    public List<InputKey> AllowedInputs;
}
