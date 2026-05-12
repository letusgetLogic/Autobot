public enum TurnLockState
{
    Idle,

    // --- shop phase ---
    WaitingForTurnHash,
    WaitingForTurnAck,

    // --- battle phase ---
    BattleSimulating,
    WaitingForBattleResultHash,
    WaitingForBattleResultAck,

    Confirmed,
    Failed,
}

