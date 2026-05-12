using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

[DisallowMultipleComponent]
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    public Lobby CurrentLobby { get; private set; }
    public bool IsHost { get; private set; }

    public event Action<string> OnRelayReady; // relay join code
    public event Action<string> OnError;

    private const int MaxPlayers = 2;
    private const string RelayJoinCodeKey = "RelayJoinCode";
    private const string GameVersionKey = "GameVersion";
    private const string GameVersion = "0.1";

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

    private void Start()
    {
        _ = InitializeAsync();
        _ = CreateLobbyAsync();
        
    }

    /// <summary>
    /// Initialize Unity Services and authenticate the player anonymously.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Create a new lobby as host and set up Relay.
    /// </summary>
    /// <returns></returns>
    public async Task CreateLobbyAsync()
    {
        try
        {
            IsHost = true;

            var lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        GameVersionKey,
                        new DataObject(DataObject.VisibilityOptions.Public, GameVersion)
                    }
                }
            };

            CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(
                "AutoBattler Lobby",
                MaxPlayers,
                lobbyOptions
            );

            // Create Relay allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Save Relay join code in lobby
            await LobbyService.Instance.UpdateLobbyAsync(
                CurrentLobby.Id,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            RelayJoinCodeKey,
                            new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                        }
                    }
                }
            );

            OnRelayReady?.Invoke(joinCode);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Join an existing lobby by its ID and retrieve the Relay join code.
    /// </summary>
    /// <param name="_lobbyId"></param>
    /// <returns></returns>
    public async Task JoinLobbyAsync(string _lobbyId)
    {
        try
        {
            IsHost = false;

            CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(_lobbyId);

            if (!CurrentLobby.Data.ContainsKey(RelayJoinCodeKey))
            {
                OnError?.Invoke("Lobby has no Relay join code.");
                return;
            }

            string joinCode = CurrentLobby.Data[RelayJoinCodeKey].Value;
            OnRelayReady?.Invoke(joinCode);
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
        }
    }

    /// <summary>
    /// Query available lobbies that match the game version.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Lobby>> QueryLobbiesAsync()
    {
        try
        {
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(
                new QueryLobbiesOptions
                {
                    Count = 10
                }
            );

            return response.Results;
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            return new List<Lobby>();
        }
    }

    /// <summary>
    /// Leave the current lobby, deleting it if host or removing self if client.
    /// </summary>
    /// <returns></returns>
    public async Task LeaveLobbyAsync()
    {
        if (CurrentLobby == null)
            return;

        try
        {
            if (IsHost)
            {
                await LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Id);
            }
            else
            {
                await LobbyService.Instance.RemovePlayerAsync(
                    CurrentLobby.Id,
                    AuthenticationService.Instance.PlayerId
                );
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
        }
        finally
        {
            CurrentLobby = null;
        }
    }
}
