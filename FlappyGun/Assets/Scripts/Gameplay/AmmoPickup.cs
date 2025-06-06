using UnityEngine;

/// <summary>
/// A marker component to identify an object as an ammo pickup.
/// The actual logic is handled by the PlayerController upon collision.
/// </summary>
public class AmmoPickup : MonoBehaviour
{
    // --- To be assigned in Inspector ---
    [Header("Effects")]
    [Tooltip("VFX to play when the ammo is collected.")]
    public GameObject collectionEffectPrefab;
    // public AudioClip collectionSound; // For when you add sound
    
    // No logic needed here anymore.
} 