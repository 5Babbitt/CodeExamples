using HeathenEngineering.SteamworksIntegration;
using System;
using System.Collections.Generic;
using Unity.Netcode;

public class GameEvents
{
    public Action OnLoadGameScene = delegate { }; // On Entering a game scene
    public Action OnGameStart = delegate { }; // After start countdown
    public Action OnGameOver = delegate { }; // On Train Destroyed/Passengers out
    public Action OnGameFinished = delegate { }; // On Reach the station
    public Action<bool> OnGameSystemsToggled = delegate { };
}

public class PlayerEvents
{
    public Action<List<ulong>> SpawnPlayers = delegate { };
    public Action AllPlayersReady = delegate { };
    public Action<NetworkObject> OnRemotePlayerSpawn = delegate { };
}

public class MultiplayerEvents
{
    public event Action OnLobbyJoined = delegate { };
    public event Action OnLeftLobby = delegate { };
    public event Action<UserData> OnPlayerJoined = delegate { };
    public event Action<UserData> OnPlayerLeft = delegate { };
}