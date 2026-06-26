using UnityEngine;

/// <summary>
/// Isometric camera controller with smooth pan, zoom, and rotation.
/// Attach to the main camera or a camera rig parent.
/// </summary>
public sealed class IsometricCameraController : MonoBehaviour
{
    [Header("Pan")]
    [SerializeField] private float panSpeed      = 12f;
    [SerializeField] private float panSmooth      = 8f;
    [SerializeField] private bool  edgePanEnabled = true;
    [SerializeField] private float edgePanMargin  = 20f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed   = 3f;
    [SerializeField] private float zoomSmooth  = 6f;
    [SerializeField] private float zoomMin     = 4f;
    [SerializeField] private float zoomMax     = 20f;
    [SerializeField] private float zoomDefault = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotateSmooth = 6f;

    [Header("Bounds")]
    [SerializeField] private Vector2 boundsMin = new Vector2(-20f, -20f);
    [SerializeField] private Vector2 boundsMax = new Vector2(20f, 20f);

    [Header("Camera Reference")]
    [SerializeField] private Camera targetCamera;

    private Vector3 targetPosition;
    private float   targetZoom;
    private float   targetAngle;
    private float   currentAngle;
    private float   tiltAngle;
    private bool    isDragging;
    private Vector3 dragOrigin;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponentInChildren<Camera>();
        }

        targetPosition = transform.position;
        targetZoom     = zoomDefault;
        tiltAngle      = transform.eulerAngles.x;
        targetAngle    = transform.eulerAngles.y;
        currentAngle   = targetAngle;
    }

    private void Update()
    {
        HandleKeyboardPan();
        HandleMousePan();
        HandleEdgePan();
        HandleZoom();
        HandleRotation();
        ApplySmoothing();
    }

    private void HandleKeyboardPan()
    {
        var h = 0f;
        var v = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    v += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  v -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  h -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h += 1f;

        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            return;
        }

        var forward = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;
        var right   = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.right;

        targetPosition += (right * h + forward * v).normalized * (panSpeed * Time.deltaTime);
        ClampPosition();
    }

    private void HandleMousePan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (!isDragging)
        {
            return;
        }

        var delta = (Input.mousePosition - dragOrigin) * 0.01f;
        dragOrigin = Input.mousePosition;

        var forward = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;
        var right   = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.right;

        targetPosition -= (right * delta.x + forward * delta.y) * panSpeed * 0.3f;
        ClampPosition();
    }

    private void HandleEdgePan()
    {
        if (!edgePanEnabled || !Application.isFocused)
        {
            return;
        }

        var mouse = Input.mousePosition;
        var h = 0f;
        var v = 0f;

        if (mouse.x < edgePanMargin)                  h -= 1f;
        if (mouse.x > Screen.width - edgePanMargin)   h += 1f;
        if (mouse.y < edgePanMargin)                  v -= 1f;
        if (mouse.y > Screen.height - edgePanMargin)  v += 1f;

        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            return;
        }

        var forward = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;
        var right   = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.right;

        targetPosition += (right * h + forward * v).normalized * (panSpeed * 0.7f * Time.deltaTime);
        ClampPosition();
    }

    private void HandleZoom()
    {
        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, zoomMin, zoomMax);
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetAngle -= 90f;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            targetAngle += 90f;
        }
    }

    private void ApplySmoothing()
    {
        var dt = Time.deltaTime;

        // Position
        transform.position = Vector3.Lerp(transform.position, targetPosition, panSmooth * dt);

        // Rotation
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotateSmooth * dt);
        transform.rotation = Quaternion.Euler(tiltAngle, currentAngle, 0f);

        // Zoom (orthographic size or position-based)
        if (targetCamera != null && targetCamera.orthographic)
        {
            targetCamera.orthographicSize = Mathf.Lerp(targetCamera.orthographicSize, targetZoom, zoomSmooth * dt);
        }
        else if (targetCamera != null)
        {
            // For perspective, move camera along its forward axis
            var camLocal = targetCamera.transform.localPosition;
            var dist = Mathf.Lerp(camLocal.magnitude, targetZoom, zoomSmooth * dt);
            targetCamera.transform.localPosition = camLocal.normalized * dist;
        }
    }

    private void ClampPosition()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, boundsMin.x, boundsMax.x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, boundsMin.y, boundsMax.y);
    }

    /// <summary>Snap camera to a specific world position instantly.</summary>
    public void FocusOn(Vector3 worldPosition)
    {
        targetPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
    }

    /// <summary>Reset zoom to default level.</summary>
    public void ResetZoom()
    {
        targetZoom = zoomDefault;
    }
}
