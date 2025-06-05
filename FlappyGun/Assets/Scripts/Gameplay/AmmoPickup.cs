using UnityEngine;

// Corresponds to "Recharge" (the ammo pickup item)
public class AmmoPickup : MonoBehaviour
{
    // public AudioClip pickupSound; // Assign in Inspector
    // public GameObject pickupEffectPrefab; // Assign in Inspector for VFX

    // According to README, these are fixed and appear at random positions/intervals.
    // Spawning logic would likely be in a separate manager or GameManager.

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider is the Player
        if (collision.gameObject.CompareTag("Player")) // Ensure Player GameObject has "Player" tag
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReloadAmmo(); // Call the ReloadAmmo method on the PlayerController

                // Play sound and/or visual effect for pickup
                // if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                // if (pickupEffectPrefab != null) Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                Debug.Log("Ammo picked up by player.");

                Destroy(gameObject); // Destroy the ammo pickup object after collection
            }
        }
    }

    // TODO: Add logic for spawning these pickups. This could be done by an AmmoPickupSpawner script
    // or integrated into the GameManager or another dedicated spawner manager.
    // The README mentions: "Des sprites de "chargeurs" apparaissent à l'écran à des positions aléatoires
    // et à des intervalles de temps réguliers. Ces sprites de chargeurs sont fixes (ne se déplacent pas)."
} 