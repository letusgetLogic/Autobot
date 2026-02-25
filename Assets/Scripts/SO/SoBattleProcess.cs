using UnityEngine;

[CreateAssetMenu(fileName = "BattleProcess", menuName = "ScriptableObject/BattleProcess")]
public class SoBattleProcess : ScriptableObject
{
    public float RefreshRate = 3.0f;

    [Range(0f, 2f)] public float DurationInit = 0.1f;

    [Range(0f, 2f)] public float DurationCheckOutcome = 1.0f;

    [Range(0f, 2f)] public float DurationInsert = 0.5f;

    [Range(0f, 2f)] public float DurationAttack = 0.5f;

    [Range(0f, 2f)] public float DurationHandleEachAbility = 2.0f;

    [Range(0f, 2f)] public float DurationShutdown = 0.5f;

    [Range(0f, 2f)] public float DurationBattleOver = 1.0f;

    [Range(0f, 2f)] public float WaitForClickShow = 5.0f;

    [Tooltip("Duration instantiate an unit")]
    [Range(0f, 2f)] public float DurationShootOut = 0.1f;

    [Range(0f, 2f)] public float DurationShowDescription = 1.5f;

    [Range(0f, 2f)] public float DurationShowCollide = 1.5f;

    [Range(0f, 2f)] public float DelaySteal = 0.3f;
}
