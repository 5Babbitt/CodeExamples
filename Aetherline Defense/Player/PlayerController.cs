using HeathenEngineering.SteamworksIntegration;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [field: SerializeField] public InputReader input { get; private set; }

    // TODO Move to a dedicated script later
    [SerializeField] private GameObject playerModel;
    [SerializeField] private PlayerPortraitPopup portrait;

    private UserData steamData;
    public UserData PlayerSteamData => steamData;

    // Network Events
    public event Action<bool> TogglePlayerSystems = delegate { }; // Enable or disable player functions

    public readonly PlayerControlEvents Events = new();

    public void Init(UserData user)
    {
        Debug.Log($"{user.Name} Initialised");
        steamData = user;

        steamData.LoadAvatar((result) => 
        {
            portrait.SetPortrait(steamData.Name, steamData.Avatar);
            TogglePlayerSystems.Invoke(false);
        });
    }

    public override void OnNetworkSpawn()
    {
        Events.OnSpawn.Invoke(IsOwner);

        if (!IsOwner)
        {
            TogglePlayer(false);
            enabled = false;
            return;
        }

        Debug.Log($"{name} {OwnerClientId} Player Spawned");
    }

    public void TogglePlayer(bool value, bool bypassNetwork = false)
    {
        if (!bypassNetwork) 
        {
            if (!IsServer || !IsOwner)
                return;
        }
            
        Debug.Log($"Toggled Player {OwnerClientId}: {value}");

        if (value)
            input.Enable();

        playerModel.SetActive(value);

        TogglePlayerSystems.Invoke(value);
    }

    [ContextMenu("Toggle Player")]
    public void TogglePlayerTrue()
    {
        TogglePlayer(true, true);
    }
}

public class PlayerControlEvents
{
    public Action<bool> OnSpawn = delegate { }; // Send if player is the owner

    // Movement
    public Action<MoveState> SetMoveState = delegate { };

    // Animation
    public Action<string, bool> SetAnimatonBool = delegate { };
    public Action<string, float> SetAnimatonFloat = delegate { };
    public Action<string> SetAnimatonTrigger = delegate { };
}