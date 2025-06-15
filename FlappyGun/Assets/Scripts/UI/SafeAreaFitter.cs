using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform panel;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        if (panel == null)
        {
            panel = GetComponent<RectTransform>();
        }

        // Protect against division by zero which creates NaN values
        if (Screen.width <= 0 || Screen.height <= 0)
        {
            Debug.LogWarning("SafeAreaFitter: Screen dimensions are invalid, skipping safe area application");
            return;
        }

        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // Safe division with validation
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Validate the calculated values to prevent NaN
        if (float.IsNaN(anchorMin.x) || float.IsNaN(anchorMin.y) || 
            float.IsNaN(anchorMax.x) || float.IsNaN(anchorMax.y))
        {
            Debug.LogError("SafeAreaFitter: Calculated NaN values, reverting to defaults");
            anchorMin = Vector2.zero;
            anchorMax = Vector2.one;
        }

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
        
        //Debug.LogFormat("Applied safe area: Min({0}, {1}) Max({2}, {3})", anchorMin.x, anchorMin.y, anchorMax.x, anchorMax.y);
    }
} 