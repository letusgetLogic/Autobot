using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObject/Unit")]
public class SoUnit :ScriptableObject
{
    public Sprite Sprite;

    public string 
        Name,
        Description;

    public int 
        Health,
        Damage,
        Cost;
    
    public TriggerType TriggerType;
    public DoType DoType;
}
