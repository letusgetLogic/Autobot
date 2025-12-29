using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayTransportBootstrap : MonoBehaviour
{
    public static RelayTransportBootstrap Instance;

    public NetworkDriver Driver { get; private set; }
    public NetworkConnection Connection { get; private set; }

    public bool IsHost { get; private set; }

    public event Action OnConnected;
    public event Action<string> OnError;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!Driver.IsCreated)
            return;

        Driver.ScheduleUpdate().Complete();

        if (IsHost)
            AcceptConnections();
        else
            CheckClientConnection();
    }

    private void OnDestroy()
    {
        if (Driver.IsCreated)
            Driver.Dispose();
    }

    /// <summary>
    /// Connect to Relay as Host or Client based on LobbyManager state.
    /// </summary>
    /// <param name="_relayJoinCode"></param>
    public async void Connect(string _relayJoinCode)
    {
        try
        {
            if (LobbyManager.Instance.IsHost)
                await StartHostAsync(_relayJoinCode);
            else
                await StartClientAsync(_relayJoinCode);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Start Relay as Host.
    /// </summary>
    /// <param name="_joinCode"></param>
    /// <returns></returns>
    private async Task StartHostAsync(string _joinCode)
    {
        IsHost = true;

        Allocation allocation =
    await RelayService.Instance.CreateAllocationAsync(1);

        var relayServerData = new RelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.ConnectionData,
            allocation.ConnectionData, 
            allocation.Key,
            isSecure: true
        );

        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);
        
        Driver = NetworkDriver.Create(settings);

        var endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = 0;

        Driver.Bind(endpoint);
        Driver.Listen();

        OnConnected?.Invoke();
    }

    /// <summary>
    /// Start Relay as Client.
    /// </summary>
    /// <param name="_joinCode"></param>
    /// <returns></returns>
    private async Task StartClientAsync(string _joinCode)
    {
        IsHost = false;

        JoinAllocation join =
     await RelayService.Instance.JoinAllocationAsync(_joinCode);

        var relayServerData = new RelayServerData(
            join.RelayServer.IpV4,
            (ushort)join.RelayServer.Port,
            join.AllocationIdBytes,
            join.ConnectionData,
            join.HostConnectionData,
            join.Key,
            isSecure: true
        );

        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        Driver = NetworkDriver.Create(settings);

        Connection = Driver.Connect(NetworkEndpoint.AnyIpv4);

        OnConnected?.Invoke();
    }

    /// <summary>
    /// Accept incoming client connections (Host only).
    /// </summary>
    private void AcceptConnections()
    {
        NetworkConnection incoming;
        while ((incoming = Driver.Accept()) != default)
        {
            Connection = incoming;
            Debug.Log("Client connected");
        }
    }

    /// <summary>
    /// Check client connection status (Client only).
    /// </summary>
    private void CheckClientConnection()
    {
        if (!Connection.IsCreated)
            return;

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = Connection.PopEvent(Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Connected to host");
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Disconnected from host");
                Connection = default;
            }
        }
    }

}
