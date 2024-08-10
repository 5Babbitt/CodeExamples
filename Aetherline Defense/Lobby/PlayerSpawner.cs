using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Add to a gameobject in any game scene. Add empty transforms as spwanpoints
/// </summary>
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] spawnPoints;

    public event Action<NetworkObject> OnPlayerSpawn;
    public event Action<List<PlayerController>> OnAllPlayersSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerManager.Events.SpawnPlayers += SpawnAllPlayers;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        PlayerManager.Events.SpawnPlayers -= SpawnAllPlayers;
    }

    public void SpawnAllPlayers(List<ulong> clients)
    {
        if (!IsServer)
            return;

        var players = new List<PlayerController>();

        foreach (ulong id in clients)
        {
            PlayerController _player;

            if (spawnPoints.Length == 0)
                _player = Instantiate(player).GetComponent<PlayerController>();
            else
                _player = Instantiate(player, spawnPoints[(int)id].position, Quaternion.identity).GetComponent<PlayerController>();

            var netPlayer = _player.GetComponent<NetworkObject>();

            netPlayer.SpawnAsPlayerObject(id, destroyWithScene: true);
            _player.Init(MultiplayerManager.Instance.GetLobbyMembers[(int)id].user); // Call the initialization function passing the Steam ID

            players.Add(_player);

            Debug.Log(_player.ToString());

            OnPlayerSpawn.Invoke(netPlayer);
        }

        OnAllPlayersSpawned.Invoke(players);
        Debug.Log($"{name}: All Players Spawned");
    }

    public void SpawnPlayer(ulong clientID)
    {

    }
}
