using UnityEngine;

/// <summary>
/// Slowly orbits the camera around a focus point for a cinematic
/// main menu background effect. Attach to the main menu camera.
/// </summary>
public sealed class MenuCameraOrbit : MonoBehaviour
{
    [SerializeField] private Vector3 focusPoint = new Vector3(1f, 1f, 1.5f);
    [SerializeField] private float orbitSpeed   = 3f;  // degrees per second
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobSpeed     = 0.4f;

    private float currentAngle;
    private float orbitRadius;
    private float baseHeight;

    private void Awake()
    {
        var offset = transform.position - focusPoint;
        orbitRadius = new Vector2(offset.x, offset.z).magnitude;
        baseHeight = transform.position.y;
        currentAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        currentAngle += orbitSpeed * Time.deltaTime;
        var rad = currentAngle * Mathf.Deg2Rad;

        var x = focusPoint.x + Mathf.Cos(rad) * orbitRadius;
        var z = focusPoint.z + Mathf.Sin(rad) * orbitRadius;
        var y = baseHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;

        transform.position = new Vector3(x, y, z);
        transform.LookAt(focusPoint + Vector3.up * 1.2f);
    }
}
