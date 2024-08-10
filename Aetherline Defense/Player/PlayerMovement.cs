using UnityEngine;

public class PlayerMovement : PlayerSystem
{
    private CharacterController controller;
    private Camera cam;

    [Header("Move Settings")]
    [SerializeField] private MoveState moveState;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private float rotationSpeed;

    [Header("Climb Settings")]
    [SerializeField] private float climbSpeed;

    [Header("Gravity Settings")]
    [SerializeField] private float gravityForce;
    [SerializeField] private Vector3 currentGravity;

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    protected override void SubscribeEvents()
    {
        player.TogglePlayerSystems += ToggleMovement;
        player.Events.SetMoveState += SetMoveState;
    }

    protected override void UnsubscribeEvents()
    {
        player.TogglePlayerSystems -= ToggleMovement;
        player.Events.SetMoveState -= SetMoveState;

    }

    private void ToggleMovement(bool value)
    {

    }

    private void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        switch (moveState)
        {
            case MoveState.defaultState:
                Movement();
                Gravity();
                break;
            case MoveState.ladderState:
                ClimbLadder();
                break;
            case MoveState.interactingState:
                
                break;
        }


    }

    public void SetMoveState(MoveState newState)
    {
        moveState = newState;
        Debug.Log($"Player Current Move state: {moveState} : {newState}");
    }

    private void Movement()
    {
        moveInput = IsOwner ? player.input.Move : moveInput;
        if (!IsSpawned)
        {
            moveInput = player.input.Move;
        }

        Vector3 forwardMoveVector = cam.transform.forward * moveInput.y;
        Vector3 horizontalMoveVector = cam.transform.right * moveInput.x;
        moveVector = Vector3.ProjectOnPlane((forwardMoveVector + horizontalMoveVector).normalized, Vector3.up) * moveSpeed;

        playerAnimator.SetFloat("moveSpeed", moveVector.magnitude);
        
        controller.Move(moveVector * Time.deltaTime);
        Rotate(moveVector.normalized);
    }

    private void Gravity()
    {
        Vector3 gravityVector = Vector3.down * gravityForce;
        Vector3 downwardForce = gravityVector * Time.deltaTime;

        controller.Move(downwardForce);
    }

    private void Rotate(Vector3 vector)
    {
        if (vector == Vector3.zero)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), Time.deltaTime * rotationSpeed);
    }

    private void ClimbLadder()
    {
        //facing direction (0,0,-1)
        //Vector3 climbVector = Vector3.up * moveInput.y;
        //moveVector = (climbVector).normalized * moveSpeed;

        moveInput = IsOwner ? player.input.Move : moveInput;
        if (!IsSpawned)
        {
            moveInput = player.input.Move;
        }

        Vector3 UpMoveVector = Vector3.up * moveInput.y;
        // climbing animation
        // Let Ladder Script Control Movement
        controller.Move(UpMoveVector * moveSpeed/2 * Time.deltaTime);
    }
}

public enum MoveState
{
    defaultState,
    ladderState,
    interactingState
}
