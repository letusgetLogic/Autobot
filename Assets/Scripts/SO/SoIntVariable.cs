using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObject/IntVariable")]
public class SoIntVariable : ScriptableObject
{
    public int Value {  get; set; }
}
