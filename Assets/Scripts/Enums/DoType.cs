public enum DoType
{
    None = 0,

    Buff = 1,

    ShootOut = 2,

    ShutDown = 3,

    Steal = 400, // The great value represent the higher priority to execute before other abilities.

    Debuff = 5,

    CopyHealth,
    BuffSelfIfAFriendLv3,

    BuffFriendIfLost,
    BuffShopUnitPernament,

    Deal,

    GainCoin,
    GainPerk,

    Stock,
    ReplaceFood,

    CopyAbility,


    Discount,

    Swallow,

    FriendAheadRepeatsHisAbility,
}
