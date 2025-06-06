using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- To be assigned in Inspector ---
    public Transform playerTarget; // The player's Transform to follow
    
    // --- Camera Follow settings ---
    [Header("Camera Follow")]
    [SerializeField] private float smoothSpeed = 0.125f; // How quickly the camera catches up
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Set Y to 0 for direct centering

    // --- Camera Shake settings ---
    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.1f;
    [SerializeField] private float shakeMagnitude = 0.2f;

    // --- Private variables ---
    private Vector3 velocity = Vector3.zero;
    private Vector3 originalPosition;
    private float currentShakeDuration = 0f;

    void LateUpdate()
    {
        if (playerTarget == null)
        {
            Debug.LogWarning("CameraController: Player target not assigned!");
            return;
        }

        // --- Camera Follow Logic ---
        Vector3 desiredPosition = playerTarget.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        
        // --- Apply Camera Shake ---
        if (currentShakeDuration > 0)
        {
            smoothedPosition += Random.insideUnitSphere * shakeMagnitude;
            currentShakeDuration -= Time.deltaTime;
        }

        transform.position = smoothedPosition;
    }

    // Public method to trigger the shake
    public void TriggerShake()
    {
        currentShakeDuration = shakeDuration;
    }
} 