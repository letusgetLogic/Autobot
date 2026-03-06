public enum NetMsgType : byte
{
    Hello,        // handshake
    Ready,        // both players ready

    TurnInput,    // shop / actions
    TurnHash,
    TurnAck,
    Endturn,

    StartBattle,  // contains seed
    BattleResultHash,
    BattleResultAck,

    ReconnectRequest,
    SnapshotData,
    SnapshotAck,
}
