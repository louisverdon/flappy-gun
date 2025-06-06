using UnityEngine;

public class VFXSelfDestruct : MonoBehaviour
{
    // The lifetime of the effect in seconds.
    public float lifetime = 0.5f;

    void Start()
    {
        // Schedule the destruction of this GameObject after 'lifetime' seconds.
        Destroy(gameObject, lifetime);
    }
} 