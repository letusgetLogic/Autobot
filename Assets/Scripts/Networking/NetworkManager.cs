using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetworkManager
{
    private NetworkDriver driver;
    private NetworkConnection connection;

    /// <summary>
    /// Send the player's input for the current turn to the server, along with a hash of the input for later verification.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="seed"></param>
    /// <param name="actions"></param>
    public void SendTurnInput(
        int turn,
        int seed,
        IReadOnlyList<int> actions
    )
    {
        uint hash = DeterminismHash.HashTurnInput(turn, seed, actions);

        driver.BeginSend(connection, out var writer);

        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnInput);
        writer.WriteUInt(hash);

        writer.WriteInt(turn);
        writer.WriteInt(seed);
        writer.WriteByte((byte)actions.Count);

        foreach (var a in actions)
            writer.WriteInt(a);

        driver.EndSend(writer);
    }

    /// <summary>
    /// Send a hash of the turn input for the current turn to the opponent, so they can verify it matches the hash of the input they received from the server.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="hash"></param>
    public void SendTurnHash(int turn, uint hash)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnHash);
        writer.WriteInt(turn);
        writer.WriteUInt(hash);
        driver.EndSend(writer);
    }

    /// <summary>
    /// Send an acknowledgment for the turn input hash, indicating that the client has received and verified the opponent's turn input hash for the current turn. 
    /// This allows both clients to confirm that they are in sync before proceeding to the next turn.
    /// </summary>
    /// <param name="turn"></param>
    public void SendTurnAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }

    /// <summary>
    /// Receive the turn input hash from the opponent and verify it against the hash of the turn input received from the server.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="expectedTurn"></param>
    /// <param name="expectedSeed"></param>
    /// <param name="expectedActions"></param>
    /// <returns></returns>
    public bool ReceiveTurnInput(
        DataStreamReader reader,
        int expectedTurn,
        int expectedSeed,
        IReadOnlyList<int> expectedActions
    )
    {
        uint remoteHash = reader.ReadUInt();

        uint localHash = DeterminismHash.HashTurnInput(
            expectedTurn,
            expectedSeed,
            expectedActions
        );

        return remoteHash == localHash;
    }

    /// <summary>
    /// Send a hash of the battle result for the current turn to the opponent, so they can verify it matches the hash of the battle result they received from the server.
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="hash"></param>
    public void SendBattleResultHash(int turn, uint hash)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.BattleResultHash);
        writer.WriteInt(turn);
        writer.WriteUInt(hash);
        driver.EndSend(writer);
    }

    /// <summary>
    /// Send an acknowledgment for the battle result hash, indicating that the client has received and verified the opponent's battle result hash for the current turn.
    /// </summary>
    /// <param name="turn"></param>
    public void SendBattleResultAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.BattleResultAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }

    /// <summary>
    /// Send a reconnect request to the server, including the last confirmed turn number that the client has received and processed.
    /// </summary>
    /// <param name="lastConfirmedTurn"></param>
    public void SendReconnectRequest(int lastConfirmedTurn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.ReconnectRequest);
        writer.WriteInt(lastConfirmedTurn);
        driver.EndSend(writer);
    }

    /// <summary>
    /// Server: Handle a reconnect request from a client by comparing the client's last confirmed turn number with the server's current turn number.
    /// </summary>
    /// <param name="clientTurn"></param>
    /// <param name="currentTurn"></param>
    /// <param name="latestSnapshot"></param>
    public void OnReconnectRequest(int clientTurn, int currentTurn, GameSnapshot latestSnapshot)
    {
        if (clientTurn > currentTurn)
        {
            SendDesync(clientTurn);
            return;
        }

        SendSnapshot(latestSnapshot);
    }

    /// <summary>
    /// Send a desynchronization (desync) notification to the client, indicating that the client's game state is out of sync with the server's current game state.
    /// </summary>
    /// <param name="currentTurn"></param>
    public void SendDesync(int currentTurn)
    {
        // something wrong, notify server
    }

    /// <summary>
    /// Send a game snapshot to the client, including the current turn number, random seed, player health, and unit states. 
    /// This allows the client to update its game state to match the server's current game state after a reconnect or desync.
    /// </summary>
    /// <param name="snapshot"></param>
    public void SendSnapshot(GameSnapshot snapshot)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.SnapshotData);

        writer.WriteInt(snapshot.Turn);
        writer.WriteUInt(snapshot.Seed);

        writer.WriteInt(snapshot.Player1Health);
        writer.WriteInt(snapshot.Player2Health);

        //WriteUnits(writer, snapshot.Player1Units);
        //WriteUnits(writer, snapshot.Player2Units);

        driver.EndSend(writer);
    }

    private void WriteUnits(DataStreamWriter writer/* unit data struct * , UnitState[] units*/)
    {
        //writer.WriteInt(units.Length);
        //foreach (var u in units)
        //{
        //    writer.WriteInt(u.UnitId);
        //    writer.WriteInt(u.Health);
        //    writer.WriteInt(u.Attack);
        //    writer.WriteByte((byte)(u.Alive ? 1 : 0));
        //}
    }

    // Client: Apply snapshot

    //public void OnSnapshotReceived(GameSnapshot snapshot)
    //{
    //    BattleState.LoadSnapshot(snapshot);

    //    turnManager.SetTurn(snapshot.Turn);
    //    turnManager.ResetLockstep();

    //    net.SendSnapshotAck(snapshot.Turn);
    //}

    /// <summary>
    /// Send an acknowledgment for the received game snapshot, indicating that the client has successfully received and applied the snapshot for the current turn.
    /// </summary>
    /// <param name="turn"></param>
    public void SendSnapshotAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.SnapshotAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }

}
