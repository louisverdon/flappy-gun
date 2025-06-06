using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading scene

// Corresponds to "Conditions de Victoire et Défaite", "Score et Progression", etc.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public UIManager uiManager; // Assign in Inspector or find dynamically
    public EnemySpawner enemySpawner; // Will be found dynamically
    public AmmoSpawner ammoSpawner; // Will be found dynamically
    public Camera mainCamera; // Will be found dynamically

    public enum GameState { StartMenu, Playing, Paused, GameOver }
    public GameState currentState { get; private set; }

    // public AudioClip gameOverSound; // Assign in Inspector
    // public AudioClip gameStartSound; // Assign in Inspector
    // private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // GameManager persists across scenes
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

    public void StartGame()
    {
        score = 0;
        if (uiManager != null) uiManager.UpdateScore(score);
        if (enemySpawner != null) enemySpawner.StartSpawning();
        if (ammoSpawner != null) ammoSpawner.StartSpawning();
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

        if (enemySpawner != null) enemySpawner.StopSpawning();
        if (ammoSpawner != null) ammoSpawner.StopSpawning();
        ChangeState(GameState.GameOver);
        // if (gameOverSound != null && audioSource != null) audioSource.PlayOneShot(gameOverSound);
        Debug.Log("Game Over! Final Score: " + score);
        // TODO: Stop enemy spawning (this is now handled by StopSpawning)
        // TODO: Disable player controls (PlayerController should check GameManager.currentState)
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