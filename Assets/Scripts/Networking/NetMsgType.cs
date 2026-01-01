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


    //       Hello = 1,
    //StartGame = 2,
    //PlayerCommand = 3,
    //EndTurn = 4,
    //CombatSeed = 5,
    //TurnHash = 6,
    //ResyncRequest = 7,
    //Disconnect = 8

}
