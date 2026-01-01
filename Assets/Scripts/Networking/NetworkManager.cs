using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetworkManager
{
    private NetworkDriver driver;
    private NetworkConnection connection;

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
    public void SendTurnHash(int turn, uint hash)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnHash);
        writer.WriteInt(turn);
        writer.WriteUInt(hash);
        driver.EndSend(writer);
    }

    public void SendTurnAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }

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
    public void SendBattleResultHash(int turn, uint hash)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.BattleResultHash);
        writer.WriteInt(turn);
        writer.WriteUInt(hash);
        driver.EndSend(writer);
    }

    public void SendBattleResultAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.BattleResultAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }
    public void SendReconnectRequest(int lastConfirmedTurn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.ReconnectRequest);
        writer.WriteInt(lastConfirmedTurn);
        driver.EndSend(writer);
    }

    public void OnReconnectRequest(int clientTurn, int currentTurn, GameSnapshot latestSnapshot)
    {
        if (clientTurn > currentTurn)
        {
            SendDesync(clientTurn);
            return;
        }

        SendSnapshot(latestSnapshot);
    }


    public void SendDesync(int currentTurn)
    {
        // something wrong, notify server
    }

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

    public void SendSnapshotAck(int turn)
    {
        driver.BeginSend(connection, out var writer);
        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.SnapshotAck);
        writer.WriteInt(turn);
        driver.EndSend(writer);
    }

}
