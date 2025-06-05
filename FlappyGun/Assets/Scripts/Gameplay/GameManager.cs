using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading scene

// Corresponds to "Conditions de Victoire et Défaite", "Score et Progression", etc.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public UIManager uiManager; // Assign in Inspector or find dynamically

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

    public void StartGame()
    {
        score = 0;
        if (uiManager != null) uiManager.UpdateScore(score);
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

        ChangeState(GameState.GameOver);
        // if (gameOverSound != null && audioSource != null) audioSource.PlayOneShot(gameOverSound);
        Debug.Log("Game Over! Final Score: " + score);
        // TODO: Stop enemy spawning (EnemySpawner should check GameManager.currentState)
        // TODO: Disable player controls (PlayerController should check GameManager.currentState)
    }

    public void Replay()
    {
        // Reload the current scene to restart the game
        // This assumes your main game is in one scene. Adjust if using multiple scenes for menu vs game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // StartGame(); // StartGame will be called by the new GameManager instance in the reloaded scene via its Start() -> ChangeState(StartMenu)
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