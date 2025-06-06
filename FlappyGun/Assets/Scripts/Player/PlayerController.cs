using UnityEngine;

// Corresponds to "Contrôle du Revolver" and "Gravité" in README.md
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float recoilForce = 10f;
    [Tooltip("The maximum speed the player can reach.")]
    public float maxSpeed = 15f;

    [Header("Shooting")]
    public float fireRate = 0.5f; // Time between shots
    private float nextFireTime = 0f;
    public int maxAmmo = 6;
    private int currentAmmo;

    // TODO: Assign these in the Inspector or find them
    public Transform gunBarrel; // Point from where bullets are fired
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab; // Assign the MuzzleFlash prefab in Inspector
    public CameraController mainCameraController; // Assign the Main Camera in Inspector
    // public AudioClip shootSound;
    // public AudioClip emptyClipSound;
    // public AudioClip reloadSound;
    // private AudioSource audioSource;


    void Start()
    {
        currentAmmo = maxAmmo;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Ensure the Rigidbody never sleeps to guarantee collision/trigger detection.
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
        // audioSource = GetComponent<AudioSource>();
        // TODO: Initialize player state, link to GameManager for game over conditions
    }

    void Update()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            HandleAiming();
            HandleInput();
        }
        ApplyGravity(); // Gravity should apply even if paused to fall to ground on game over
    }

    void FixedUpdate()
    {
        // Limit the player's velocity to the maxSpeed.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            // By normalizing the velocity and multiplying by maxSpeed, we maintain the direction of movement.
            // Using sqrMagnitude is a performance optimization as it avoids a square root calculation.
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void HandleAiming()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        transform.right = direction;
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Or touch input
        {
            if (Time.time >= nextFireTime)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
                else
                {
                    // Play empty clip sound
                    if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.emptyClipSound);
                    Debug.Log("Clic à vide!");
                }
            }
        }
    }

    void Shoot()
    {
        // 1. Determine touch point P
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        touchPosition.z = 0; // Ensure it's on the 2D plane

        // 2. Orient revolver to point P -> This is now handled in HandleAiming() in Update

        // 3. Fire bullet towards P
        if (bulletPrefab != null && gunBarrel != null)
        {
            // Instantiate Muzzle Flash
            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, gunBarrel.position, gunBarrel.rotation, gunBarrel);
            }
            else
            {
                // This message will appear if the prefab is not assigned in the Inspector
                Debug.LogError("ERREUR : Le Muzzle Flash Prefab n'est pas assigné sur l'objet Player !");
            }

            GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                Vector2 fireDirection = (touchPosition - gunBarrel.position).normalized;
                bulletScript.direction = fireDirection;
            }
        }
        
        // 4. Apply recoil force
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 recoilDirection = (transform.position - touchPosition).normalized;
            rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
        }

        // Trigger Camera Shake
        if (mainCameraController != null)
        {
            mainCameraController.TriggerShake();
        }

        currentAmmo--;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.shootSound);
        Debug.Log("Bang! Ammo left: " + currentAmmo);

        // TODO: Update UI for ammo (if decided to show it)
    }

    void ApplyGravity()
    {
        // Assuming Rigidbody2D handles gravity (set Gravity Scale in Inspector)
        // Or apply custom gravity force:
        // GetComponent<Rigidbody2D>().AddForce(Vector2.down * gravityStrength * Time.deltaTime);
    }

    public void ReloadAmmo()
    {
        currentAmmo = maxAmmo;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.reloadSound);
        Debug.Log("Reloaded! Ammo: " + currentAmmo);
        // TODO: Play reload VFX/SFX
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with Enemy - Game Over");
            GameManager.Instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("Ground")) // Assuming ground has "Ground" tag
        {
            Debug.Log("Player hit the ground - Game Over");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.groundImpactSound);
            GameManager.Instance.GameOver();
        }
        // Collision with AmmoPickup is handled by AmmoPickup's OnTriggerEnter2D
    }

    // Add this method to handle trigger collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ammo"))
        {
            // --- DIAGNOSTIC ---
            // If you see this message, the tag comparison is working.
            Debug.Log("SUCCESS: Collision with Ammo object confirmed inside the IF block.");

            // Reload ammo.
            ReloadAmmo();
            
            // Play VFX if the component and prefab exist
            AmmoPickup ammoPickup = other.GetComponent<AmmoPickup>();
            if (ammoPickup != null && ammoPickup.collectionEffectPrefab != null)
            {
                Instantiate(ammoPickup.collectionEffectPrefab, other.transform.position, Quaternion.identity);
            }

            // Destroy the ammo pickup object.
            Destroy(other.gameObject);
        }
    }
} 