using UnityEngine;

/// <summary>
/// PlayerInteractor
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    private Camera cam;
    public PlayerController player {  get; private set; }

    [Header("Interaction Settings")]
    [SerializeField] private LayerMask interactionLayers;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private float interactOffset;
    [SerializeField] private float interactHeight;
    [SerializeField] private float interactRadius;

    [SerializeField] private Interactable currentInteractable;
    private Vector3 forwardVector;
    private Vector3 upVector;

    public Transform humanTransform;
    public CarrySlot carrySlot;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        humanTransform = GetComponentInChildren<HumanMovement>().transform;
        cam = Camera.main;

    }

    private void Update()
    {
        forwardVector = (Application.isPlaying ? Vector3.ProjectOnPlane(cam.transform.forward, humanTransform.up).normalized : humanTransform.forward) * interactOffset;
        upVector = humanTransform.up * interactHeight;

        var interactables = Physics.OverlapSphere(humanTransform.position + forwardVector + upVector, interactRadius, interactionLayers);

        if (interactables.Length > 0)
        {
            var interactable = interactables[0];
            if (currentInteractable == null || interactable.gameObject != currentInteractable.gameObject)
            {
                interactable.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnFocus();
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    void OnSwitch()
    {

    }

    public Carriable GetCarriable()
    {
        return carrySlot.carryItem;
    }

    public void SetCarriable(Carriable carriable)
    {
        carrySlot.carryItem = carriable;
    }

    public void ClearInteractable()
    {
        currentInteractable = null;
    }

    void HandleInteract()
    {
        if (!canInteract) 
            return;

        if (currentInteractable == null)
        {
            if (carrySlot.carryItem != null)
            {
                carrySlot.Drop();
            }
            return;
        }

        if (player.CurrentActivePlayer is HumanMovement)
        {
            Debug.Log("Human Interact");

            currentInteractable.OnInteract(this);

        }
        else if (player.CurrentActivePlayer is ShadowMovement)
        {
            Debug.Log("Shadow Interact");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (!Application.isPlaying )
        {
            forwardVector = (Application.isPlaying ? Vector3.ProjectOnPlane(cam.transform.forward, humanTransform.up).normalized : humanTransform.forward) * interactOffset;
            upVector = humanTransform.up * interactHeight;
        }

        Gizmos.DrawWireSphere(humanTransform.position + forwardVector + upVector, interactRadius);
    }
}
