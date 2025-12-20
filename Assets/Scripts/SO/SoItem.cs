using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class SoItem :ScriptableObject
{
    public Sprite Sprite;

    public string ID;
    public string Name;
    public string Description;

    public SoIntVariable Cost;

    public TriggerType TriggerType;
    public DoType DoType;
}
