using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports;
using Steamworks;
using System;
using HeathenEngineering.SteamworksIntegration.API;
using Unity.Services.Lobbies.Models;

public class MultiplayerManager : Singleton<MultiplayerManager>
{
    [SerializeField] private SteamNetworkingSocketsTransport steamTransport;

    [Header("Lobby Settings")]
    [SerializeField] private int playerSlots;
    [SerializeField] private bool isPrivate;

    [Space(20)]
    [SerializeField] private string lobbyID;

    [Header("Server Settings")]
    [SerializeField] private ulong serverID;

    private LobbyData currentSession;

    public event Action OnLobbyJoined = delegate { };
    public event Action OnLeftLobby = delegate { };
    public event Action<UserData> OnPlayerJoined = delegate { };
    public event Action<UserData> OnPlayerLeft = delegate { };

    public string GetJoinCode => (currentSession != null) ? currentSession.HexId : null;
    public int GetPlayerCount => (currentSession != null) ? currentSession.MemberCount : 0;
    public bool IsConnectedToLobby => currentSession != null;
    public bool IsConnectedToServer => NetworkManager.Singleton.IsConnectedClient;
    public bool IsLobbyOwner => IsConnectedToLobby && (currentSession.Owner.user == UserData.Me);
    public LobbyMemberData[] GetLobbyMembers => (currentSession != null) ? currentSession.Members : null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        steamTransport = NetworkManager.Singleton.GetComponent<SteamNetworkingSocketsTransport>();
    }

    private void SubscribeLobbyEvents()
    {
        Matchmaking.Client.EventLobbyGameCreated.AddListener(OnGameServerSet);
        Matchmaking.Client.EventLobbyChatUpdate.AddListener(OnLobbyMembersUpdate);
        OnLobbyJoined.Invoke();
    }

    private void UnsubscribeLobbyEvents()
    {
        Matchmaking.Client.EventLobbyGameCreated.RemoveListener(OnGameServerSet);
        Matchmaking.Client.EventLobbyChatUpdate.RemoveListener(OnLobbyMembersUpdate);
    }

    private void InitLobby(LobbyData lobby)
    {
        currentSession = lobby;
        lobbyID = lobby.HexId;

        Debug.Log($"Created lobby:  {lobby.Name}");
        Debug.Log($"Max Players:    {lobby.MaxMembers}");
        Debug.Log($"Lobby Type:     {lobby.Type}");
        Debug.Log($"Lobby ID:       {lobby.HexId}");
    }

    private void OnCreatedLobby(LobbyData lobby)
    {
        lobby.Name = $"{UserData.Me.Name}'s Lobby";

        InitLobby(lobby);
        SubscribeLobbyEvents();
    }

    private void OnJoinedLobby(LobbyData lobby)
    {
        InitLobby(lobby);
        SubscribeLobbyEvents();
    }

    private void OnLobbyMembersUpdate(LobbyChatUpdate_t user)
    {
        if (user.m_ulSteamIDLobby == currentSession)
        {
            var player = UserData.Get(user.m_ulSteamIDUserChanged);
            var state = (EChatMemberStateChange)user.m_rgfChatMemberStateChange;

            if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
            {
                Debug.Log($"{player.Name} joined the lobby!");
                OnPlayerJoined.Invoke(player);
            }
            else
            {
                Debug.Log($"{player.Name} left the lobby!");
                OnPlayerLeft.Invoke(player);
            }
        }
    }

    private void OnGameServerSet(LobbyGameCreated_t lobbyGame)
    {
        Debug.Log("Server Started");
        
        steamTransport.ConnectToSteamID = lobbyGame.m_ulSteamIDGameServer;

        if (NetworkManager.Singleton.IsConnectedClient)
            return;
            
        StartClient();
    }

    public void CreateLobby(bool isPrivate = false)
    {
        ELobbyType lobbyType = isPrivate ? ELobbyType.k_ELobbyTypePrivate : ELobbyType.k_ELobbyTypeFriendsOnly;

        LobbyData.CreateSession(lobbyType, playerSlots, (result, lobby, ioError) => OnCreatedLobby(lobby));
    }

    public void JoinLobby(string _lobbyID)
    {
        Debug.Log("Lobby Exists: " + LobbyData.Get(_lobbyID) != null);

        try
        {
            LobbyData.Join(_lobbyID, (enteredLobby, ioError) => OnJoinedLobby(enteredLobby.Lobby));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void LeaveLobby()
    {
        UnsubscribeLobbyEvents();

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        currentSession.Leave();
        currentSession = null;

        OnLeftLobby.Invoke();
    }

    public void StartHost()
    {
        if (currentSession == null)
        {
            Debug.LogError("No Lobby Session Detected!");
            return;
        }

        if (!IsLobbyOwner)
        {
            Debug.LogError("Not Lobby Owner!");
            return;
        }

        steamTransport.ConnectToSteamID = currentSession.Owner.user;

        NetworkManager.Singleton.StartHost();
        currentSession.SetGameServer();

        serverID = currentSession.GameServer.id.m_SteamID;
    }

    private void StartClient()
    {
        if (currentSession == null)
        {
            Debug.LogError("No Lobby Session Detected");
            return;
        }

        NetworkManager.Singleton.StartClient();

        serverID = currentSession.GameServer.id.m_SteamID;
    }

    [ContextMenu("Join Steam Lobby")]
    private void ContextJoinLobby() => JoinLobby(lobbyID);

    [ContextMenu("Create Steam Lobby")]
    private void ContextCreateLobby() => CreateLobby(isPrivate);

    [ContextMenu("Leave Steam Lobby")]
    private void ContextLeaveLobby() => LeaveLobby();

    [ContextMenu("Start Host")]
    private void ContextStartHost() => StartHost();
}
