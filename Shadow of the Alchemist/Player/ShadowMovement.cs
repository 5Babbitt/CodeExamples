using Cinemachine;
using FiveBabbittGames;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ShadowMovement
/// </summary>
public class ShadowMovement : PlayerMovement
{
    private Rigidbody rb;
    bool isValid;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetNormal;
    [SerializeField] private Quaternion targetRotation;
    private Quaternion lastTargetRotation;

    [Header("Shadow Settings")]
    [SerializeField] private bool canSwitch;
    [SerializeField] private bool canSwitchCrouched;

    [Header("Arc Cast Settings")]
    [SerializeField] float arcAngle = 270;
    [SerializeField] int arcResolution = 8;
    [SerializeField] private float arcRadius = 5;
    [SerializeField] private LayerMask groundLayers;

    [Header("Check Settings")]
    [SerializeField] private float checkRadius = 0.4f;
    [SerializeField] private float checkHeight = 0.4f;

    [Header("NavMesh Settings")]
    [SerializeField] private float navMeshSampleDistance = 2f;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        Camera.main.GetComponent<CinemachineBrain>().m_WorldUpOverride = transform;
    }

    private void OnEnable()
    {
        if (!PhysicsUtils.ArcCast(transform.position, transform.rotation, arcAngle, arcRadius * Time.fixedDeltaTime, arcResolution, groundLayers, out RaycastHit hit))
        {
            Debug.Log("Arc Cast False On Switch");
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000, groundLayers))
            {
                Debug.Log("Ground Found Below");
                transform.position = hit.point;
            }
        }

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        Debug.Log("Switched to shadow");
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleRotate(moveVector.normalized);
        HandleMove();
    }

    protected override void HandleMove()
    {
        float radius = arcRadius * moveVector.normalized.magnitude * Time.fixedDeltaTime;
        Quaternion rotation = moveVector != Vector3.zero ? Quaternion.LookRotation(moveVector.normalized, transform.up) : transform.rotation;

        // This uses the PhysicsUtils Arc Cast from my custom tools package, as the name implies it casts a ray several times to create an arc
        if (PhysicsUtils.ArcCast(transform.position, rotation, arcAngle, radius, arcResolution, groundLayers, out RaycastHit hit))
        {
            targetPosition = hit.point;
            targetNormal = hit.normal;
            isValid = true;
        }
        else
        {
            isValid = false;
        }

        rb.MovePosition(MathUtils.LerpByDistance(transform.position, targetPosition, Mathf.Clamp01(moveSpeed)));
    }

    protected override void HandleRotate(Vector3 vector)
    {
        if (vector == Vector3.zero)
        {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, lastTargetRotation, rotationSpeed * Time.fixedDeltaTime));
            return;
        }

        Vector3 forwardDirection = Vector3.ProjectOnPlane(moveVector.normalized, targetNormal);
        targetRotation = Quaternion.LookRotation(forwardDirection.normalized, targetNormal);
        lastTargetRotation = targetRotation;
        var currentRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(currentRotation);
    }

    protected override void HandleGravity()
    {

    }

    protected override void Move(Vector3 value)
    {
        moveVector = Vector3.ProjectOnPlane(value.normalized, transform.up) * moveSpeed;
    }

    public bool CanSwitch(bool drawGizmos = false)
    {
        Vector3 samplePosition;
        if (FindNearestClearPoint(out samplePosition))
        {
            for (int i = 0; i < 3; i++)
            {
                if (drawGizmos)
                {
                    Gizmos.DrawWireSphere(samplePosition + ((Vector3.up * 1.001f) * checkHeight * ((i * 2) + 1)), checkRadius);
                }

                if (Physics.CheckSphere(samplePosition + ((Vector3.up * 1.001f) * checkHeight * ((i * 2) + 1)), checkRadius, groundLayers))
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public bool FindNearestClearPoint(out Vector3 samplePosition)
    {
        samplePosition = Vector3.zero;
        NavMeshHit navMeshHit;

        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

        if (NavMesh.SamplePosition(transform.position, out navMeshHit, navMeshSampleDistance, walkableMask))
        {
            samplePosition = navMeshHit.position;
            return true;
        }

        return false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + moveVector.normalized / 2);
        Gizmos.DrawSphere(transform.position + moveVector.normalized / 2, 0.05f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.right));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up));

        Quaternion rotation = moveVector != Vector3.zero ? Quaternion.LookRotation(moveVector.normalized, transform.up) : transform.rotation;

        Gizmos.color = isValid ? Color.yellow : Color.cyan;
        PhysicsUtils.ArcCast(transform.position, rotation, arcAngle, moveSpeed, arcResolution, groundLayers, out RaycastHit hit, drawGizmos: true);
        Gizmos.DrawSphere(hit.point, 0.05f);
        Gizmos.DrawLine(hit.point, hit.point + targetNormal.normalized);

        Gizmos.color = Color.magenta;
        CanSwitch(true);
    }
}
