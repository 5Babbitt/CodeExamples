using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pushable
/// </summary>
[RequireComponent(typeof(PlayerInput), typeof(Rigidbody))]
public class Pushable : Interactable
{
    [Header("Pushable Settings")]
    public Transform pushObject;
    public Vector3 objectBounds;
    public float forwardDot;
    public float rightDot;
    public bool isPushing;

    [Header("Movement Settings")]
    public Vector3 moveDirection;
    public float moveInput;
    public float pushSpeed;
    public float playerOffset;

    HumanMovement human;
    Rigidbody rb;

    public override void Start()
    {
        base.Start();
        pushObject = transform.GetChild(0);
        rb = GetComponent<Rigidbody>();
        objectBounds = pushObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
    }

    private void FixedUpdate()
    {
        if (!isPushing)
            return;

        Vector3 moveVector = moveDirection * moveInput * pushSpeed * Time.fixedDeltaTime;

        //rb.MovePosition(transform.position + moveVector);
        rb.velocity = moveVector;
    }

    public override void OnInteract(PlayerInteractor interactingPlayer)
    {
        if (!isPushing)
        {
            base.OnInteract(interactingPlayer);

            human = (HumanMovement)interactingPlayer.player.CurrentActivePlayer;

            human.SetCanMove(false);
            human.transform.position = SnapPlayerPosition(human.transform);

            interactingPlayer.transform.SetParent(pushObject);

            rb.isKinematic = false;

            moveDirection = (pushObject.position - human.transform.position);
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

            isPushing = true;
        }
        else
        {
            isPushing = false;

            interactingPlayer.transform.SetParent(null);
            human.SetCanMove(true);
            human = null;

            moveDirection = Vector3.zero;
            moveInput = 0;
            rb.isKinematic = true;
            rb.velocity = moveDirection;
        }
    }

    Vector3 SnapPlayerPosition(Transform playerTransform)
    {
        Vector3 playerDirection = (pushObject.position - playerTransform.position).normalized;
        Vector3 offset;

        forwardDot = Vector3.Dot(pushObject.forward, playerDirection);
        rightDot = Vector3.Dot(pushObject.right, playerDirection);
        Debug.Log($"Right Dot: {rightDot} \nForward Dot: {forwardDot}");

        if (Mathf.Abs(forwardDot) >= Mathf.Abs(rightDot))
        {
            float multiplier = (forwardDot > 0) ? 1 : -1;
            offset = pushObject.forward * (objectBounds.z + playerOffset) * multiplier;
        }
        else
        {
            float multiplier = (rightDot > 0) ? 1 : -1;
            offset = pushObject.right * (objectBounds.x + playerOffset) * multiplier;
        }

        Vector3 playerPosition = pushObject.position - offset;

        playerPosition = new Vector3(playerPosition.x, 0, playerPosition.z);

        return playerPosition;
    }

    void OnMove(InputValue input)
    {
        if (!isPushing)
            return;

        moveInput = input.Get<Vector2>().y;
    }

    void OnInteract(InputValue input)
    {
        return;
    }

    private void OnDrawGizmos()
    {
        if (pushObject == null)
            return;

        objectBounds = pushObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pushObject.position + (pushObject.forward * objectBounds.z), 0.2f);
        Gizmos.DrawSphere(pushObject.position - (pushObject.forward * objectBounds.z), 0.2f);
        Gizmos.DrawSphere(pushObject.position + (pushObject.right * objectBounds.x), 0.2f);
        Gizmos.DrawSphere(pushObject.position - (pushObject.right * objectBounds.x), 0.2f);
    }

    protected override void UpdateInteractText()
    {
        useText = "Push";

        base.UpdateInteractText();
    }
}