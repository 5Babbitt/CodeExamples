using Unity.Netcode;
using UnityEngine;

public abstract class PlayerSystem : NetworkBehaviour
{
    protected PlayerController player;
    protected PlayerAnimator playerAnimator;

    protected virtual void Awake()
    {
        player = transform.root.GetComponent<PlayerController>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    protected virtual void OnEnable()
    {
        SubscribeEvents();
    }

    protected virtual void OnDisable()
    {
        UnsubscribeEvents();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        player.Events.OnSpawn += (bool isOwner) => enabled = isOwner;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        player.Events.OnSpawn -= (bool isOwner) => enabled = isOwner;
    }

    protected abstract void SubscribeEvents();
    protected abstract void UnsubscribeEvents();
}
