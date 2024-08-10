using FiveBabbittGames;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    Camera cam;

    [Header("Input Settings")]
    [SerializeField] private Vector2 moveInput;

    [Header("Player Settings")]
    public bool isShadow;
    [SerializeField] private HumanMovement human;
    [SerializeField] private ShadowMovement shadow;
    [SerializeField] private PlayerInteractor interactor;
    public PlayerInteractor Interactor => interactor;

    private PlayerMovement activePlayerMovement;
    public PlayerMovement CurrentActivePlayer => activePlayerMovement;

    protected override void Awake()
    {
        base.Awake();

        human = GetComponentInChildren<HumanMovement>();
        shadow = GetComponentInChildren<ShadowMovement>();
        interactor = GetComponent<PlayerInteractor>();
    }

    void Start()
    {
        cam = Camera.main;

        Switch(!isShadow);
    }

    void Update()
    {
        CalculateMoveVector(moveInput);
    }

    void CalculateMoveVector(Vector2 move)
    {
        Vector3 verticalVector = cam.transform.forward * move.y;
        Vector3 horizontalVector = cam.transform.right * move.x;
        Vector3 moveVector = (verticalVector + horizontalVector);

        BroadcastMessage("Move", moveVector.normalized);
    }

    public void SwitchToHuman()
    {
        Switch(false);
    }

    void Switch(bool value)
    {
        if (isShadow && !shadow.CanSwitch())
        {
            Debug.Log("Unable to switch to human here");
            return;
        }
        else if (!isShadow && !human.CanMove)
        {
            Debug.Log("Unable to switch to shadow while pushing an object");

            return;
        }

        if (value)
        {
            if (shadow.FindNearestClearPoint(out Vector3 clearPosition))
            {
                human.transform.position = clearPosition;
            }
            else
            {
                human.transform.position = shadow.transform.position;
            }

            interactor.ClearInteractable();
            interactor.enabled = true;
        }
        else
        {
            shadow.transform.position = human.transform.position;

            interactor.carrySlot.Drop();
            interactor.ClearInteractable();
            interactor.enabled = false;

            if (HUDController.Instance != null)
                HUDController.Instance.SetInteractText();
        }

        isShadow = !value;

        human.gameObject.SetActive(!isShadow);
        shadow.gameObject.SetActive(isShadow);

        activePlayerMovement = isShadow ? shadow : human;
    }

    void Interact()
    {
        Debug.Log("Interact");
        BroadcastMessage("HandleInteract");
    }

    void Crouch()
    {
        Debug.Log("Crouch");
        BroadcastMessage("HandleCrouch");
    }

    void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    void OnCrouch(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Crouch();
        }
    }

    void OnInteract(InputValue inputValue)
    {
        Interact();
    }

    void OnSwitch(InputValue inputValue)
    {
        Switch(isShadow);
    }
}
