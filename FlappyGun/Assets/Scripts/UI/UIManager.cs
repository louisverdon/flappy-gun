using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Text, Button, Panel
using TMPro; // <-- AJOUTER CETTE LIGNE

// Corresponds to "Interface Utilisateur (UI)"
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI Panels")]
    public GameObject startScreenPanel; // Panel for the start screen
    public GameObject gameOverPanel;    // Panel for the game over screen
    public GameObject gameplayHudPanel; // The parent panel for all gameplay HUD elements (score, ammo, etc.)

    [Header("Gameplay UI")]
    public GameObject scoreInfoObject;           // Parent object for score icon and text
    public GameObject ammoInfoObject;            // Parent object for ammo icon and text
    public GameObject magazineInfoObject;        // Parent object for magazine icon and text
    public TextMeshProUGUI scoreText;            // For displaying current score during gameplay
    public TextMeshProUGUI ammoText;             // For displaying current ammo
    public TextMeshProUGUI magazinesText;        // For displaying remaining magazines
    public GameObject reloadHintObject;           // To prompt the player to reload

    [Header("Screen Texts")]
    public TextMeshProUGUI highScoreText;        // To display the high score on the start screen
    public TextMeshProUGUI finalScoreText;       // Text on game over panel to show final score
    
    [Header("Buttons")]
    public Button replayButton;       // Button on game over panel to replay
    public Button startButton;        // Button on start screen panel to start game

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        
        UpdateAmmoUI(0, 0);
        ShowReloadHint(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }

    public void UpdateAmmoUI(int currentAmmo, int magazines)
    {
        if (ammoText != null)
        {
            ammoText.text = "" + currentAmmo;
        }
        if (magazinesText != null)
        {
            magazinesText.text = "" + magazines;
        }
    }

    public void ShowReloadHint(bool show)
    {
        if (reloadHintObject != null)
        {
            reloadHintObject.SetActive(show);
        }
    }

    public void ShowStartScreen()
    {
        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameplayHudPanel != null) gameplayHudPanel.SetActive(false);
        if (reloadHintObject != null) reloadHintObject.SetActive(false);

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
         if (gameplayHudPanel != null) gameplayHudPanel.SetActive(true);
         if (highScoreText != null) highScoreText.gameObject.SetActive(false); // Hide high score during gameplay
         Debug.Log("UI: Showing Game HUD");
    }

    public void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (startScreenPanel != null) startScreenPanel.SetActive(false);
        if (gameplayHudPanel != null) gameplayHudPanel.SetActive(false);
        if (reloadHintObject != null) reloadHintObject.SetActive(false);
        
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