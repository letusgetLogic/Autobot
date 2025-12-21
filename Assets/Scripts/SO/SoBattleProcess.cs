using UnityEngine;

[CreateAssetMenu(fileName = "BattleProcess", menuName = "ScriptableObject/BattleProcess")]
public class SoBattleProcess : ScriptableObject
{
    [Range(0f, 2f)] public float DurationInit = 0.1f;

    [Range(0f, 2f)] public float DurationCheckOutcome = 1.0f;

    [Range(0f, 2f)] public float DurationInsert = 0.5f;

    [Range(0f, 2f)] public float DurationAttack = 0.5f;

    [Range(0f, 2f)] public float DurationDelaySummon = 0.1f;

    [Range(0f, 2f)] public float DurationHideAbilityDescription = 1.5f;

    [Range(0f, 2f)] public float DurationHandleEachAbility = 2.0f;

    [Range(0f, 2f)] public float DurationShutdown = 0.5f;

    [Range(0f, 2f)] public float DurationBattleOver = 1.0f;
}
