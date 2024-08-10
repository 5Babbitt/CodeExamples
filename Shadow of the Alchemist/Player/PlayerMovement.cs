using UnityEngine;

/// <summary>
/// PlayerMovement
/// </summary>
public abstract class PlayerMovement : MonoBehaviour
{
    protected PlayerController player;

    [Header("Move Settings")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotationSpeed;
    protected Vector3 moveVector;

    protected virtual void Awake()
    {
        player = transform.root.GetComponent<PlayerController>();
    }

    protected abstract void Move(Vector3 value);
    protected abstract void HandleMove();
    protected abstract void HandleGravity();
    protected abstract void HandleRotate(Vector3 vector);
}
