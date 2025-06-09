using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Text, Button, Panel
using TMPro; // <-- AJOUTER CETTE LIGNE

// Corresponds to "Interface Utilisateur (UI)"
public class UIManager : MonoBehaviour
{
    // Assign these in the Unity Inspector
    public TextMeshProUGUI scoreText;            // For displaying current score during gameplay
    public TextMeshProUGUI highScoreText;        // To display the high score on the start screen
    public GameObject startScreenPanel; // Panel for the start screen
    public GameObject gameOverPanel;    // Panel for the game over screen
    public TextMeshProUGUI finalScoreText;       // Text on game over panel to show final score
    public Button replayButton;       // Button on game over panel to replay
    public Button startButton;        // Button on start screen panel to start game

    void Start()
    {
        // Ensure correct panels are shown/hidden at start, based on GameManager state
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentState == GameManager.GameState.StartMenu)
            {
                ShowStartScreen();
            }
            else if (GameManager.Instance.currentState == GameManager.GameState.Playing)
            {
                ShowGameHUD();
            }
            else if (GameManager.Instance.currentState == GameManager.GameState.GameOver)
            {
                ShowGameOverScreen(GameManager.Instance.score); // Show game over if starting in that state (e.g. after scene reload)
            }
        }
        else
        {
            ShowStartScreen(); // Default if GameManager isn't ready yet
        }

        // Hook up button listeners
        if (replayButton != null && GameManager.Instance != null)
        {
            replayButton.onClick.RemoveAllListeners(); // Clear existing listeners to prevent duplicates on scene reload
            replayButton.onClick.AddListener(GameManager.Instance.Replay);
        }
        if (startButton != null && GameManager.Instance != null)
        {
            startButton.onClick.RemoveAllListeners(); // Clear existing listeners
            startButton.onClick.AddListener(GameManager.Instance.StartGame);
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void ShowStartScreen()
    {
        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false); // Hide score during start screen

        // Display the high score
        if (highScoreText != null && GameManager.Instance != null)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "Meilleur Score: " + GameManager.Instance.highScore;
        }
        
        Debug.Log("UI: Showing Start Screen");
    }

    public void ShowGameHUD()
    {
         if (startScreenPanel != null) startScreenPanel.SetActive(false);
         if (gameOverPanel != null) gameOverPanel.SetActive(false);
         if (scoreText != null) scoreText.gameObject.SetActive(true); // Show score
         if (highScoreText != null) highScoreText.gameObject.SetActive(false); // Hide high score during gameplay
         Debug.Log("UI: Showing Game HUD");
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (startScreenPanel != null) startScreenPanel.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false); // Hide in-game score during game over
        
        // Display final score and high score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + finalScore;
        }
        if (highScoreText != null && GameManager.Instance != null)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "Meilleur Score: " + GameManager.Instance.highScore;
        }

        Debug.Log("UI: Showing Game Over Screen");
    }
} 