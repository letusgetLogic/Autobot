using UnityEngine;

[CreateAssetMenu(fileName = "Bot", menuName = "ScriptableObject/Bot")]
public class SoUnit :ScriptableObject
{
    public Sprite Sprite;

    public int Health;
    public int Damage;
    
    public TriggerType TriggerType;
    public DoType DoType;
}
