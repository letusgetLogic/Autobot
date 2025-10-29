using UnityEngine;

[CreateAssetMenu(fileName = "BattleProcess", menuName = "ScriptableObject/BattleProcess")]
public class SoBattleProcess : ScriptableObject
{
    [RangeAttribute(0f, 1f)] public float DurationInit = 0.1f;
    [RangeAttribute(0f, 1f)] public float DurationCheckOutcome = 1.0f;
    [RangeAttribute(0f, 1f)] public float DurationInsert = 0.5f;
    [RangeAttribute(0f, 1f)] public float DurationAttack = 0.5f;
    [RangeAttribute(0f, 1f)] public float DurationEachAbility = 1.0f;
    [RangeAttribute(0f, 1f)] public float DurationFaint = 1.0f;
    [RangeAttribute(0f, 1f)] public float DurationBattleOver = 1.0f;
}
