using UnityEngine;

// Corresponds to "Tir" (the projectile itself)
public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Vector2 direction; // Set by Player/Gun when instantiated
    public float lifetime = 3f; // Time in seconds before bullet is destroyed if it hits nothing

    void Start()
    {
        // Destroy the bullet after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet based on its direction and speed
        // Using transform.Translate for simplicity in 2D if not using Rigidbody2D for bullets
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bullet collided with an enemy
        if (collision.gameObject.CompareTag("Enemy")) // Ensure enemies have the "Enemy" tag
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(); // Tell the enemy script it was hit
            }
            Destroy(gameObject); // Destroy the bullet immediately upon hitting an enemy
        }
        // Optional: Destroy bullet if it hits other colliders like a "Wall" or "Boundary"
        // else if (collision.gameObject.CompareTag("Wall"))
        // {
        //     Destroy(gameObject);
        // }
    }
} 