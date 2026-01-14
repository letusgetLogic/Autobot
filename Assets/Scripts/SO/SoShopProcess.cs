using UnityEngine;

[CreateAssetMenu(fileName = "ShopProcess", menuName = "ScriptableObject/ShopProcess")]
public class SoShopProcess : ScriptableObject
{
    [Tooltip("Delay showing turn")]
    [Range(0f, 2f)] public float DelayShowingTurn = 0.5f;

    [Tooltip("Duration showing turn")]
    [Range(0f, 2f)] public float DurationShowingTurn = 0.5f;

    [Tooltip("Delay showing charging of the bot on charging slot at start")]
    [Range(0f, 2f)] public float DelayChargingAtStart = 0.5f;

    [Tooltip("Duration of charging the bots on team and charging slots")]
    [Range(0f, 2f)] public float DurationCharging = 0.5f;

    [Tooltip("Delay starting the battle after charging")]
    [Range(0f, 2f)] public float DelayStartBattleAfterCharging = 1f;

    [Tooltip("Delay pushing other bot to make space")]
    [Range(0f, 2f)] public float DelayPushing = 0.5f;

    [Tooltip("Delay pushing other bot to make space, while a fusion between 2 bots is possible")]
    [Range(0f, 2f)] public float DelayPushingFusion = 1f;

    [Tooltip("Delay hiding description after activating ability")]
    [Range(0f, 2f)] public float DelayHideDescription = 1.5f;

    [Tooltip("Delay summon")]
    [Range(0f, 2f)] public float DurationDelaySummon = 0.1f;

}
