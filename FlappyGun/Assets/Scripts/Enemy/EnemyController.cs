using UnityEngine;

// Corresponds to "Ennemis (\"Flappy Birds\")" - movement and interaction
public class EnemyController : MonoBehaviour
{
    public float horizontalSpeed = 2f;
    public float verticalJumpForce = 5f;
    public float jumpInterval = 1.5f;
    private float jumpTimer;
    private Rigidbody2D rb;
    private int direction = 1; // 1 for right, -1 for left

    [Header("Effects")]
    public GameObject deathEffectPrefab; // Assign in Inspector for VFX

    // public AudioClip deathSound; // Assign in Inspector for SFX
    // private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // audioSource = GetComponent<AudioSource>(); // Or use a central audio manager
        jumpTimer = Random.Range(0, jumpInterval); // Randomize first jump

        // Determine initial direction based on spawn side (set by Spawner)
        // For now, let's assume it's set by spawner or defaults
    }

    public void SetDirection(int dir) {
        direction = Mathf.Clamp(dir, -1, 1); // Ensure dir is only 1 or -1

        // Flip sprite based on direction
        if (direction != 0) // Don't flip if direction is neutral
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * direction;
            transform.localScale = newScale;
        }
    }

    void Update()
    {
        // Horizontal Movement
        rb.linearVelocity = new Vector2(horizontalSpeed * direction, rb.linearVelocity.y);

        // Vertical "Bond" Movement
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpInterval)
        {
            rb.AddForce(Vector2.up * verticalJumpForce, ForceMode2D.Impulse);
            jumpTimer = 0f;
        }

        // TODO: Destroy if off-screen (e.g., if it exits opposite side without being shot)
    }

    public void TakeDamage()
    {
        // Play elimination effects
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // This message will appear if the prefab is not assigned in the Inspector
            Debug.LogError("ERREUR : Le Death Effect Prefab n'est pas assigné sur le PREFAB de l'Ennemi !");
        }
        
        Debug.Log("Enemy hit!");

        // Notify GameManager to add score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1); // Assuming 1 point per enemy
        }

        // Play death sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathSound);
        }

        Destroy(gameObject);
    }

    // Collision with player is handled by PlayerController or GameManager
} 