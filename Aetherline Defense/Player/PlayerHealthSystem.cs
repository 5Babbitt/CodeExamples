using System;
using Unity.Netcode;
using UnityEngine;

// Simplest design route is probably:
// - NetworkVariable for health with an OnValueChange function
// - if the server sees that health is less than 0, send a ClientRpc to hide the player, wait for the countdown, teleport them and unhide them

public class PlayerHealthSystem : PlayerSystem
{
    [SerializeField] private int maxHealth;

    // Network Variables
    [SerializeField] private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public void TakeDamage(int value) => ChangeHealth(-value);
    public void Heal(int value) => ChangeHealth(value);
    public int PlayerHealth => currentHealth.Value;

    public Action<ulong> OnDeath;

    public CountdownTimer respawnTimer;
    [SerializeField] private float respawnTime;

    protected override void Awake()
    {
        base.Awake();
        
        respawnTimer = new CountdownTimer(respawnTime);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        currentHealth.Value = maxHealth;
    }

    protected override void SubscribeEvents()
    {
        currentHealth.OnValueChanged += HealthChanged;
        respawnTimer.OnTimerStop += RespawnPlayer;
    }

    protected override void UnsubscribeEvents()
    {
        currentHealth.OnValueChanged -= HealthChanged;
        respawnTimer.OnTimerStop -= RespawnPlayer;
    }

    private void Update()
    {
        if (respawnTimer.IsRunning)
        {
            respawnTimer.Tick(Time.deltaTime);
            // Debug.Log("Timer Progress: " + TimerUtils.FloatToTime(respawnTimer.Progress * respawnTime));
        }
    }

    private void ChangeHealth(int value)
    {
        ChangeHealthServerRPC(value);
    }

    [ServerRpc]
    private void ChangeHealthServerRPC(int value)
    {
        Debug.Log("Change Health by " + value);

        currentHealth.Value += value;
    }

    public void SpawnPlayer()
    {
        if (!IsOwner)
            return;

        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        // Set Position to spawn point
        GameObject[] respawnNodes = GameObject.FindGameObjectsWithTag("Respawn");
        if (respawnNodes.Length != 0)
        {
            transform.position = respawnNodes[0].transform.position;
        }

        // Re enable Player
        player.TogglePlayer(true);
        currentHealth.Value = maxHealth;
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        if (previousValue < newValue)
        {
            Debug.Log($"Player {OwnerClientId} healed {newValue - previousValue} points");
        }

        if (previousValue > newValue)
        {
            Debug.Log($"Player {OwnerClientId} took {newValue - previousValue} damage");
        }

        if (previousValue > 0 && newValue <= 0)
        {
            // Called On player death
            Debug.Log($"Player {OwnerClientId} Died");

            // Play death animation
            playerAnimator.SetTrigger("isDead");
            playerAnimator.SetBool("isDead", true);

            //AudioManager.Instance.PlaySoundOnce(playerDeathSFX);

            // wait for end of animations
            respawnTimer.Start();
            player.TogglePlayer(false);
            // Die
        }
        else if (previousValue < 0 && newValue >= 0)
        {
            // Called on respawning player
            Debug.Log($"Player {OwnerClientId} Respawned");
            // Replenish Health

        }
    }

    [ContextMenu("Deal small Damage")]
    private void DebugDamageSmall()
    {
        ChangeHealth(-7);
    }

    [ContextMenu("Deal medium Damage")]
    private void DebugDamageMedium()
    {
        ChangeHealth(-35);
    }

    [ContextMenu("Deal large Damage")]
    private void DebugDamageLarge()
    {
        ChangeHealth(-63);
    }

    [ContextMenu("Heal")]
    private void DebugHeal()
    {
        ChangeHealth(15);
    }

    [ContextMenu("Respawn")]
    private void Respawn()
    {
        RespawnPlayer();
    }
}
