using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class SoItem :ScriptableObject
{
    public Sprite Sprite { get; set; }

    public string ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public SoIntVariable Cost { get; set; }

    public TriggerType TriggerType { get; set; }
    public DoType DoType { get; set; }
}
