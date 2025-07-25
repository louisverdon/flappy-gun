using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading scene

// Corresponds to "Conditions de Victoire et Défaite", "Score et Progression", etc.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int highScore = 0; // New variable to hold the high score
    private const string HighScoreKey = "HighScore"; // Key for PlayerPrefs

    public UIManager uiManager; // Assign in Inspector or find dynamically
    public EnemySpawner enemySpawner; // Will be found dynamically
    public AmmoSpawner ammoSpawner; // Will be found dynamically
    public Camera mainCamera; // Will be found dynamically
    public PlayerController playerController; // Will be found dynamically

    [Header("Sky Color Settings")]
    public Color skyColorAtZero = new Color(0.53f, 0.81f, 0.92f); // Light blue
    public Color skyColorAtMaxHeight = new Color(0.1f, 0.1f, 0.2f); // Dark blue/purple
    public float maxHeight = 1000f; // The height at which the sky is darkest
    public float minHeight = 0f; // The height from which the sky starts to darken
    
    public enum GameState { StartMenu, Playing, Paused, GameOver }
    public GameState currentState { get; private set; }

    private bool _adRewardPending = false;

    // public AudioClip gameOverSound; // Assign in Inspector
    // public AudioClip gameStartSound; // Assign in Inspector
    // private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // GameManager persists across scenes
            Debug.Log("GameManager: Instance initialized successfully.");
            
            // --- AdsManager Initialization ---
            // Check if an AdsManager instance already exists in the scene
            if (AdsManager.Instance == null)
            {
                // If not, create a new GameObject and add the AdsManager component
                GameObject adsManagerObject = new GameObject("AdsManager");
                adsManagerObject.AddComponent<AdsManager>();
                // The AdsManager's Awake method will handle the rest (DontDestroyOnLoad, etc.)
            }
            // --------------------------------

            // Load the high score when the game starts
            highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

            // audioSource = GetComponent<AudioSource>();
            // if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    void Start()
    {
        // Start with the main menu
        ChangeState(GameState.StartMenu);
    }

    void Update()
    {
        if (currentState == GameState.Playing && playerController != null && mainCamera != null)
        {
            float playerHeight = playerController.transform.position.y;
            
            // --- DEBUG LOGS ---
            // Debug.Log($"Player Height: {playerHeight} | Min: {minHeight}, Max: {maxHeight}");
            // ------------------

            if (playerHeight <= minHeight)
            {
                mainCamera.backgroundColor = skyColorAtZero;
            }
            else if (playerHeight >= maxHeight)
            {
                mainCamera.backgroundColor = skyColorAtMaxHeight;
            }
            else
            {
                // Interpolate because we are between min and max height
                float t = (playerHeight - minHeight) / (maxHeight - minHeight);
                mainCamera.backgroundColor = Color.Lerp(skyColorAtZero, skyColorAtMaxHeight, t);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the state to StartMenu whenever a scene is loaded.
        // This ensures that after a replay, we go back to the main menu.
        
        // We must re-find components in the newly loaded scene
        uiManager = FindFirstObjectByType<UIManager>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        ammoSpawner = FindFirstObjectByType<AmmoSpawner>();
        mainCamera = Camera.main;
        // playerController = FindFirstObjectByType<PlayerController>(); // This is now handled by RegisterPlayer

        if (ammoSpawner == null)
        {
            Debug.LogWarning("GAME MANAGER: AmmoSpawner not found in the scene. Ammo pickups will not spawn.");
        }

        if(mainCamera == null)
        {
            Debug.LogError("GAME MANAGER: Main Camera not found in the scene! Ensure a camera is tagged with 'MainCamera'.");
        }
        
        // Set the initial state, which will also handle Time.timeScale and show the correct UI
        ChangeState(GameState.StartMenu);
    }

    public void RegisterPlayer(PlayerController pc)
    {
        playerController = pc;
        Debug.Log("GameManager: Player registered successfully.");
    }

    public void StartGame()
    {
        // First, reset the player's state completely
        if (playerController != null)
        {
            playerController.ResetState();
        }

        // Then, apply the pending ad reward if available
        if (_adRewardPending)
        {
            if (playerController != null)
            {
                playerController.GrantAmmoReward(3); // Grant 3 extra magazines
                Debug.Log("Ad reward applied: +3 magazines.");
            }
            _adRewardPending = false; // Reset the flag after granting the reward
        }

        score = 0;
        if (uiManager != null) uiManager.UpdateScore(score);
        if (enemySpawner != null) enemySpawner.StartSpawning();
        if (ammoSpawner != null) ammoSpawner.StartSpawning();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.gameStartSound);
        ChangeState(GameState.Playing);
        
        // if (gameStartSound != null && audioSource != null) audioSource.PlayOneShot(gameStartSound);

        // TODO: Reset player position/state if needed
        // TODO: Ensure EnemySpawner starts/resumes
        // TODO: Clear any existing enemies/bullets from previous game
    }

    public void AddScore(int points)
    {
        if (currentState != GameState.Playing) return;
        score += points;
        if (uiManager != null) uiManager.UpdateScore(score);
    }

    public void GameOver()
    {
        if (currentState == GameState.GameOver) return; // Prevent multiple calls

        // Stop camera shake when the game ends
        if (mainCamera != null)
        {
            CameraController cc = mainCamera.GetComponent<CameraController>();
            if (cc != null)
            {
                cc.StopShake();
            }
        }

        // Check for new high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save(); // Save the new high score immediately
            Debug.Log("New High Score: " + highScore);
        }

        if (enemySpawner != null) enemySpawner.StopSpawning();
        if (ammoSpawner != null) ammoSpawner.StopSpawning();
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.gameOverSound);
        ChangeState(GameState.GameOver);
        // if (gameOverSound != null && audioSource != null) audioSource.PlayOneShot(gameOverSound);
        Debug.Log("Game Over! Final Score: " + score);
        // TODO: Stop enemy spawning (this is now handled by StopSpawning)
        // TODO: Disable player controls (PlayerController should check GameManager.currentState)
    }

    public void GrantReward()
    {
        if (currentState == GameState.StartMenu)
        {
            _adRewardPending = true;
            Debug.Log("Reward of +3 magazines will be granted on next game start.");
            // Optionally, show a message to the player on the UI
            // For example: UIManager.Instance.ShowRewardPendingMessage();
        }
    }

    public void Replay()
    {
        // Reload the current scene to restart the game
        // The OnSceneLoaded callback will handle resetting the game state.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("GameManager: State changed to " + newState);

        switch (currentState)
        {
            case GameState.StartMenu:
                Time.timeScale = 0; // Pause game logic
                if (uiManager != null) uiManager.ShowStartScreen();
                break;
            case GameState.Playing:
                Time.timeScale = 1; // Normal game speed
                if (uiManager != null) uiManager.ShowGameHUD();
                break;
            case GameState.Paused:
                Time.timeScale = 0; // Pause game logic
                // if (uiManager != null) uiManager.ShowPauseScreen(); // Optional pause screen
                break;
            case GameState.GameOver:
                Time.timeScale = 0; // Pause game logic
                if (uiManager != null) uiManager.ShowGameOverScreen(score);
                break;
        }
    }
} 