using UnityEngine;

/// <summary>
/// Employee agent with proper state machine, Animator integration,
/// and visual feedback using the design system colors.
/// States: Idle, Walking, Working, Celebrating, Panicking.
/// </summary>
public sealed class PrototypeEmployeeAgent : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private PrototypeWorkstation workstation;
    [SerializeField] private float moveSpeed   = 1.8f;
    [SerializeField] private float turnSpeed   = 12f;

    [Header("Work")]
    [SerializeField] private float workSeconds = 4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer statusLight;

    private int waypointIndex;
    private float workTimer;
    private float celebrateTimer;
    private EmployeeState state = EmployeeState.Idle;

    // Animator parameter hashes
    private static readonly int AnimIsWalking     = Animator.StringToHash("IsWalking");
    private static readonly int AnimIsWorking     = Animator.StringToHash("IsWorking");
    private static readonly int AnimIsCelebrating = Animator.StringToHash("IsCelebrating");

    public enum EmployeeState
    {
        Idle,
        Walking,
        Working,
        Celebrating,
        Panicking
    }

    public EmployeeState CurrentState => state;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        // Respect game pause
        if (TimeController.Instance != null &&
            TimeController.Instance.CurrentSpeed == TimeController.Speed.Paused)
        {
            return;
        }

        switch (state)
        {
            case EmployeeState.Idle:
                UpdateIdle();
                break;
            case EmployeeState.Walking:
                UpdateWalk();
                break;
            case EmployeeState.Working:
                UpdateWorking();
                break;
            case EmployeeState.Celebrating:
                UpdateCelebrating();
                break;
            case EmployeeState.Panicking:
                UpdatePanicking();
                break;
        }
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartWork()
    {
        if (workstation == null)
        {
            return;
        }

        state = EmployeeState.Working;
        workTimer = workSeconds;
        transform.position = workstation.WorkPoint.position;
        transform.rotation = workstation.WorkPoint.rotation;
        workstation.SetActive(true);
        SetAnimatorState(working: true);
        SetStatusColor(GameDesignConstants.EmployeeWorking);
    }

    public void Celebrate(float duration = 3f)
    {
        state = EmployeeState.Celebrating;
        celebrateTimer = duration;
        SetAnimatorState(celebrating: true);
        SetStatusColor(GameDesignConstants.EmployeeCelebrating);
    }

    public void Panic(float duration = 3f)
    {
        state = EmployeeState.Panicking;
        celebrateTimer = duration;
        SetStatusColor(GameDesignConstants.EmployeePanicking);
    }

    public void GoIdle()
    {
        state = EmployeeState.Idle;
        SetAnimatorState();
        SetStatusColor(GameDesignConstants.EmployeeIdle);
    }

    public void StartWalking()
    {
        state = EmployeeState.Walking;
        waypointIndex = 0;
        SetAnimatorState(walking: true);
        SetStatusColor(GameDesignConstants.EmployeeWalking);
    }

    public void InitializeAgent(PrototypeWorkstation station, Transform[] points)
    {
        workstation = station;
        waypoints = points;
    }

    // ── State updates ────────────────────────────────────────────────

    private void UpdateIdle()
    {
        SetStatusColor(GameDesignConstants.EmployeeIdle);

        // If we have waypoints, start walking after a brief pause
        if (waypoints != null && waypoints.Length > 0)
        {
            StartWalking();
        }
    }

    private void UpdateWalk()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            GoIdle();
            return;
        }

        var target = waypoints[waypointIndex];
        if (target == null)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            return;
        }

        var toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.04f)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;

            // Check if this waypoint is the workstation approach point
            if (workstation != null && target == workstation.ApproachPoint)
            {
                StartWork();
            }

            return;
        }

        var direction = toTarget.normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);

        var targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        SetStatusColor(GameDesignConstants.EmployeeWalking);
    }

    private void UpdateWorking()
    {
        workTimer -= Time.deltaTime;
        SetStatusColor(GameDesignConstants.EmployeeWorking);

        if (workTimer > 0f)
        {
            return;
        }

        if (workstation != null)
        {
            workstation.SetActive(false);
        }

        StartWalking();
    }

    private void UpdateCelebrating()
    {
        celebrateTimer -= Time.deltaTime;

        SetStatusColor(GameDesignConstants.EmployeeCelebrating);

        if (celebrateTimer <= 0f)
        {
            StartWalking();
        }
    }

    private void UpdatePanicking()
    {
        celebrateTimer -= Time.deltaTime;

        // Flash red
        var flash = Mathf.Sin(Time.time * 8f) > 0f
            ? GameDesignConstants.EmployeePanicking
            : GameDesignConstants.StatusCritical;
        SetStatusColor(flash);

        if (celebrateTimer <= 0f)
        {
            StartWalking();
        }
    }

    // ── Visual feedback ──────────────────────────────────────────────

    private void SetStatusColor(Color color)
    {
        if (statusLight == null)
        {
            return;
        }

        statusLight.material.color = Color.Lerp(statusLight.material.color, color, 8f * Time.deltaTime);
    }

    private void SetAnimatorState(bool walking = false, bool working = false, bool celebrating = false)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(AnimIsWalking, walking);
        animator.SetBool(AnimIsWorking, working);
        animator.SetBool(AnimIsCelebrating, celebrating);
    }
}
