using UnityEngine;

/// <summary>
/// A centralized manager for playing all sound effects in the game.
/// Implemented as a Singleton to be easily accessible from any script.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [Tooltip("Sound for when the player shoots.")]
    public AudioClip shootSound;
    [Tooltip("Sound for when the player tries to shoot with no ammo.")]
    public AudioClip emptyClipSound;
    [Tooltip("Sound for when the player picks up ammo.")]
    public AudioClip reloadSound;
    [Tooltip("Sound for when an enemy is destroyed.")]
    public AudioClip enemyDeathSound;
    [Tooltip("Sound for when the game starts.")]
    public AudioClip gameStartSound;
    [Tooltip("Sound for when the game is over.")]
    public AudioClip gameOverSound;
    [Tooltip("Sound for when the player hits the ground.")]
    public AudioClip groundImpactSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this manager persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ensure there is an AudioSource component on this object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Plays a given audio clip once.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
} 