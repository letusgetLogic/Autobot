using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class SoItem :ScriptableObject
{
    public Sprite Sprite;

    public string 
        Name,
        Description;

    public int Cost;
    
    public TriggerType TriggerType;
    public DoType DoType;
}
