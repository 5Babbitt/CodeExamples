using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

[DefaultExecutionOrder(-1)]
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PlayerSpawner _playerSpawner; // Get on load into player scene

    [Header("Player Settings")]
    [SerializeField] private List<PlayerController> _players = new List<PlayerController>();

    public List<PlayerController> Players => _players;

    // Events
    public readonly static PlayerEvents Events = new();

    private void OnEnable()
    {
        GameManager.Events.OnLoadGameScene += SpawnPlayers;
        GameManager.Events.OnGameSystemsToggled += ToggleSystems;
    }

    private void OnDisable()
    {
        GameManager.Events.OnLoadGameScene -= SpawnPlayers;
        GameManager.Events.OnGameSystemsToggled -= ToggleSystems;

        _playerSpawner.OnPlayerSpawn -= OnPlayerSpawn;
        _playerSpawner.OnAllPlayersSpawned -= OnAllPlayerSpawned;
    }

    private void SpawnPlayers()
    {
        _playerSpawner = FindObjectOfType<PlayerSpawner>();

        _playerSpawner.OnPlayerSpawn += OnPlayerSpawn;
        _playerSpawner.OnAllPlayersSpawned += OnAllPlayerSpawned;

        if (NetworkManager.Singleton.IsServer)
        {
            _playerSpawner.SpawnAllPlayers(NetworkManager.Singleton.ConnectedClientsIds.ToList());
            Debug.Log($"{name}: Players Spawned");
        }
    }

    private void ToggleSystems(bool value)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            TogglePlayersServerRPC(value);
        }
    }

    [ServerRpc]
    private void TogglePlayersServerRPC(bool value)
    {
        TogglePlayersClientRPC(value);
    }

    [ClientRpc]
    private void TogglePlayersClientRPC(bool value)
    {
        Debug.Log($"{name}: Toggled Players");

        foreach (var player in _players)
        {
            player.TogglePlayer(value);
        }
    }

    private void OnAllPlayerSpawned(List<PlayerController> playerList)
    {
        _players = playerList;
        Events.AllPlayersReady.Invoke();
    }

    private void OnPlayerSpawn(NetworkObject remotePlayerObject)
    {
        
    }
}