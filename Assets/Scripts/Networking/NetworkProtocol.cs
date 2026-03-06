using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public static class NetworkProtocol
{
    /// <summary>
    /// Send a hello message to the server.
    /// </summary>
    /// <param name="_driver"></param>
    /// <param name="_connection"></param>
    public static void SendHello(
       NetworkDriver _driver,
       NetworkConnection _connection
   )
    {
        if (!_connection.IsCreated)
            return;

        _driver.BeginSend(NetworkPipeline.Null, _connection, out var writer);

        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.Hello);

        _driver.EndSend(writer);
    }

    /// <summary>
    /// Send turn input to the server.
    /// </summary>
    /// <param name="_driver"></param>
    /// <param name="_connection"></param>
    /// <param name="_turnIndex"></param>
    /// <param name="_actions"></param>
    public static void SendTurnInput(
        NetworkDriver _driver,
        NetworkConnection _connection,
        byte _turnIndex,
        int[] _actions
    )
    {
        if (!_connection.IsCreated)
            return;

        _driver.BeginSend(NetworkPipeline.Null, _connection, out var writer);

        writer.WriteByte(NetConstants.PROTOCOL_VERSION);
        writer.WriteByte((byte)NetMsgType.TurnInput);
        writer.WriteByte(_turnIndex);
        writer.WriteByte((byte)_actions.Length);

        for (int i = 0; i < _actions.Length; i++)
            writer.WriteInt(_actions[i]);

        _driver.EndSend(writer);
    }

    /// <summary>
    /// Send start battle message to the client.
    /// </summary>
    /// <param name="_driver"></param>
    /// <param name="_connection"></param>
    /// <param name="_seed"></param>
    public static void SendStartBattle(
        NetworkDriver _driver,
        NetworkConnection _connection,
        int _seed
    )
    {
        if (!_connection.IsCreated)
            return;

        _driver.BeginSend(NetworkPipeline.Null, _connection, out var writer);
        writer.WriteByte((byte)NetMsgType.StartBattle);
        writer.WriteInt(_seed);
        _driver.EndSend(writer);
    }

    /// <summary>
    /// Processes all pending network events for the specified connection using the given network driver.
    /// </summary>
    /// <remarks>This method handles incoming data and disconnect events for the specified connection. It
    /// should be called regularly (such as once per update loop) to ensure timely processing of network events. After a
    /// disconnect event, the connection is reset to its default value.</remarks>
    /// <param name="_driver">The <see cref="NetworkDriver"/> instance used to poll and process network events.</param>
    /// <param name="_connection">The <see cref="NetworkConnection"/> for which to receive and handle events.</param>
    public static void Receive(
        NetworkDriver _driver,
        NetworkConnection _connection
    )
    {
        NetworkEvent.Type evt;
        while ((evt = _driver.PopEventForConnection(_connection, out var reader))
               != NetworkEvent.Type.Empty)
        {
            switch (evt)
            {
                case NetworkEvent.Type.Data:
                    HandleData(_driver, _connection, reader);
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client disconnected");
                    _connection = default;
                    break;
            }
        }

    }

    /// <summary>
    /// Handle incoming data packets.
    /// </summary>
    /// <param name="_reader"></param>
    private static void HandleData(
        NetworkDriver _driver,
        NetworkConnection _connection,
        DataStreamReader _reader)
    {
        byte version = _reader.ReadByte();

        if (version != NetConstants.PROTOCOL_VERSION)
        {
            Debug.LogError(
                $"Protocol mismatch. Local={NetConstants.PROTOCOL_VERSION}, Remote={version}"
            );

            _connection.Disconnect(_driver);
            return;
        }

        var msgType = (NetMsgType)_reader.ReadByte();

        switch (msgType)
        {
            case NetMsgType.Hello:
                //HandleHello(_connection);
                break;

            case NetMsgType.TurnInput:
                {
                    HandleTurnInput(_reader);
                    break;
                }
        }

        if (_reader.HasFailedReads)
        {
            Debug.LogError("Malformed packet received");
        }
    }

    /// <summary>
    /// Handle turn input received from the client. This method reads the turn index and the list of actions from the incoming data stream.
    /// </summary>
    /// <param name="_reader"></param>
    private static void HandleTurnInput(
        DataStreamReader _reader
    )
    {
        byte turn = _reader.ReadByte();
        byte count = _reader.ReadByte();

        for (int i = 0; i < count; i++)
        {
            int action = _reader.ReadInt();
            // process action
        }
        // Process turn input here
    }

}


