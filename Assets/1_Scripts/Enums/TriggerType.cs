public enum TriggerType
{
    None,

    Craft = 1,
    Recycle = 2,
    Shutdown = 3,

    LevelUp,
    BeforeAttack = 5,
    Attack = 22,
    AfterAttack = 6,
    Hurt,
    EatsFood,


    FriendSummoned,
    Tier1FriendBought,
    FourFriendsHurt,
    FriendShutdown,
    FriendAteFood,

    BeforeFriendAheadAttacks,
    FriendAheadAttacks,
    FriendAheadFaints,
    FriendAheadTriggerAbility,

    KnockOut,

    StartOfTurn,
    EndTurn = 20,
    StartOfBattle = 21,
    EndOfBattle,
}

