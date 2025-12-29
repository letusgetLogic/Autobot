public enum NetMsgType : byte
{
    Hello = 1,        // handshake
    Ready = 2,        // both players ready
    TurnInput = 3,    // shop / actions
    StartBattle = 4,  // contains seed
    Ack = 5           // optional reliability
}
