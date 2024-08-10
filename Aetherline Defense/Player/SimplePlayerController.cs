using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerController : NetworkBehaviour
{
    private CharacterController controller;
    [SerializeField] private InputReader inputReader;

    public float moveSpeed;
    public Vector3 moveVector;

    public float rotationSpeed;
    Quaternion targetRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        var moveInput = inputReader.Move;

        Vector3 forwardMoveVector = Vector3.forward * moveInput.y;
        Vector3 horizontalMoveVector = Vector3.right * moveInput.x;

        moveVector = (forwardMoveVector + horizontalMoveVector).normalized * moveSpeed * Time.deltaTime;

        controller.Move(moveVector);
        Rotate(moveVector.normalized);
    }

    private void Rotate(Vector3 vector)
    {
        if (vector == Vector3.zero)
            return;

        targetRotation = Quaternion.LookRotation(vector);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
