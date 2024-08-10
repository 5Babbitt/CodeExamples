using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader")]
public class InputReader : ScriptableObject, GameInput.IPlayerActions, GameInput.ITurretActions, GameInput.IUIActions
{
    public InputActionMap currentActionMap;

    // Player Values
    public Vector2 Move => gameInput.Player.Move.ReadValue<Vector2>();
    public bool OnInteracting => gameInput.Player.Interact.ReadValue<float>() > 0; // While true, player is holding the interact button

    public event Action OnInteractInput = delegate { };
    public event Action OnInteractCancel = delegate { };
    public event Action OnInventoryInput = delegate { };
    public event Action OnAttackInput = delegate { };

    // Turret Values
    public Vector2 TurretLook => gameInput.Turret.Turret_Look.ReadValue<Vector2>();
    public bool TurretIsFiring => gameInput.Turret.Turret_Fire.ReadValue<float>() > 0; // While true, player is holding the fire button

    public event Action OnExitTurretInput = delegate { };

    // Debug
    [Header("Player Actions")]
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool isInteracting;
    [SerializeField] private bool inventoryOpen;
    [SerializeField] private bool onAttacked;

    [Header("Turret Actions")]
    [SerializeField] private Vector2 look;
    [SerializeField] private bool isFiring;


    private static GameInput gameInput;

    void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Player.SetCallbacks(this);
            gameInput.Turret.SetCallbacks(this);
            gameInput.UI.SetCallbacks(this);
        }
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    public void Enable()
    {
        SetActionMap(gameInput.Player);
    }

    #region Action Maps
    private void SetActionMap(InputActionMap actionMap)
    {
        currentActionMap.Disable();
        currentActionMap = actionMap;
        currentActionMap.Enable();

        Debug.Log($"Current Action Map: {actionMap}");
    }

    public void SetPlayerActionMap()
    {
        SetActionMap(gameInput.Player);
    }

    public void SetTurretActionMap()
    {
        SetActionMap(gameInput.Turret);
    }

    public void SetUIActionMap()
    {
        SetActionMap(gameInput.UI);
    }
    #endregion

    // Player Actions
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAttackInput?.Invoke();
            onAttacked = true;
            onAttacked = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        isInteracting = context.ReadValue<float>() > 0 ? true : false;

        if (context.performed)
        {
            OnInteractInput?.Invoke();
        }
        else if (context.canceled)
        {
            OnInteractCancel?.Invoke();
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inventoryOpen = !inventoryOpen;
            OnInventoryInput?.Invoke();
        }
    }

    // Turret Actions
    public void OnTurret_Look(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnTurret_Fire(InputAction.CallbackContext context)
    {
        isFiring = context.ReadValue<float>() > 0 ? true : false;
    }

    public void OnTurret_Exit(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnExitTurret();
    }

    void OnExitTurret()
    {
        Debug.Log($"{GetInstanceID()} Run Once");
        OnExitTurretInput?.Invoke();
    }
}
