using UnityEngine;
using Unity.Services.LevelPlay;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    // Make these private and set them in code instead of SerializeField
    private string _appKey = "226ef3535"; // Your LevelPlay App Key
    private string _rewardedAdUnitId = "2gjv6i8xjpslyeji"; // Your Ad Unit ID
    private bool _testMode = true;

    private LevelPlayRewardedAd _rewardedAd;
    private bool _isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Log the values to ensure they're set correctly
            Debug.Log($"AdsManager created - AppKey: {_appKey}, AdUnitId: {_rewardedAdUnitId}, TestMode: {_testMode}");
            
            InitializeLevelPlay();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeLevelPlay()
    {
        Debug.Log($"AdsManager: Initializing LevelPlay SDK with App Key: {_appKey}");
        Debug.Log($"AdsManager: Platform: {Application.platform}");
        
        // Check if already initialized
        if (_isInitialized)
        {
            Debug.Log("LevelPlay already initialized");
            return;
        }
        
        // Check if values are properly set
        if (string.IsNullOrEmpty(_appKey))
        {
            Debug.LogError("AdsManager: App Key is null or empty!");
            return;
        }
        
        if (string.IsNullOrEmpty(_rewardedAdUnitId))
        {
            Debug.LogError("AdsManager: Rewarded Ad Unit ID is null or empty!");
            return;
        }
        
        Debug.Log("AdsManager: Values validated, proceeding with initialization...");
        
        // LevelPlay might not work properly in Unity Editor
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.LinuxEditor)
        {
            Debug.LogWarning("AdsManager: Running in Unity Editor - LevelPlay might not initialize properly");
            Debug.LogWarning("AdsManager: Simulating successful initialization for testing...");
            
            // Initialize immediately in editor for testing
            SimulateInitSuccess();
            return;
        }
        
        // Initialize LevelPlay SDK with simple initialization
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        
        // Initialize with app key only (simplified approach)
        Debug.Log("Calling LevelPlay.Init()...");
        try
        {
            LevelPlay.Init(_appKey);
            Debug.Log("LevelPlay.Init() called successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception during LevelPlay.Init(): {e.Message}");
        }
    }

    // Simulate successful initialization for testing in Unity Editor
    private void SimulateInitSuccess()
    {
        Debug.Log("AdsManager: Simulating LevelPlay initialization success for Editor testing");
        _isInitialized = true;
        CreateRewardedAd();
    }

    private void OnInitSuccess(LevelPlayConfiguration configuration)
    {
        Debug.Log("LevelPlay initialization successful!");
        _isInitialized = true;
        CreateRewardedAd();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError($"LevelPlay initialization failed: {error.ErrorMessage}");
        // Retry initialization after a delay
        Invoke(nameof(InitializeLevelPlay), 5f);
    }

    private void CreateRewardedAd()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("Cannot create rewarded ad: LevelPlay not initialized");
            return;
        }

        Debug.Log("Creating LevelPlay Rewarded Ad...");
        
        // In Unity Editor, we don't need to create actual LevelPlayRewardedAd
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.LinuxEditor)
        {
            Debug.Log("AdsManager: Skipping actual ad creation in Unity Editor (using simulation)");
            return;
        }
        
        // Create rewarded ad object (only on real devices)
        _rewardedAd = new LevelPlayRewardedAd(_rewardedAdUnitId);

        // Register to rewarded ad events
        _rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
        _rewardedAd.OnAdLoadFailed += OnRewardedAdLoadFailed;
        _rewardedAd.OnAdDisplayed += OnRewardedAdDisplayed;
        _rewardedAd.OnAdDisplayFailed += OnRewardedAdDisplayFailed;
        _rewardedAd.OnAdRewarded += OnRewardedAdRewarded;
        _rewardedAd.OnAdClosed += OnRewardedAdClosed;
        _rewardedAd.OnAdClicked += OnRewardedAdClicked;

        // Load the first ad
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd == null)
        {
            Debug.LogWarning("Cannot load ad: Rewarded ad not created");
            return;
        }

        Debug.Log("Loading LevelPlay Rewarded Ad...");
        _rewardedAd.LoadAd();
    }

    public void ShowRewardedAd()
    {
        Debug.Log($"ShowRewardedAd called - IsInitialized: {_isInitialized}, RewardedAd: {(_rewardedAd != null ? "Created" : "NULL")}");
        
        if (!_isInitialized)
        {
            Debug.LogWarning("Cannot show ad: LevelPlay not initialized yet");
            return;
        }
        
        // Handle Unity Editor simulation
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.LinuxEditor)
        {
            Debug.Log("AdsManager: Simulating rewarded ad in Unity Editor...");
            Debug.Log("AdsManager: Simulating user watching ad and getting reward...");
            
            // Grant reward immediately in editor for testing
            Debug.Log("AdsManager: Simulating ad reward granted!");
            
            // Grant reward to the player
            if (GameManager.Instance != null)
            {
                Debug.Log("AdsManager: Calling GameManager.GrantReward()...");
                GameManager.Instance.GrantReward();
            }
            else
            {
                Debug.LogError("AdsManager: GameManager.Instance is NULL!");
            }
            return;
        }
        
        if (_rewardedAd == null)
        {
            Debug.LogWarning("Cannot show ad: Rewarded ad not created");
            Debug.Log("Attempting to create rewarded ad...");
            CreateRewardedAd();
            return;
        }

        if (_rewardedAd.IsAdReady())
        {
            Debug.Log("Showing LevelPlay Rewarded Ad...");
            _rewardedAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready to show");
            // Try to load a new ad
            LoadRewardedAd();
        }
    }

    public bool IsRewardedAdReady()
    {
        // In Unity Editor, always return true for testing
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.LinuxEditor)
        {
            return _isInitialized;
        }
        
        return _rewardedAd != null && _rewardedAd.IsAdReady();
    }

    // Event handlers
    private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"Rewarded ad loaded successfully. Network: {adInfo.AdNetwork}");
    }

    private void OnRewardedAdLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError($"Rewarded ad failed to load: {error.ErrorMessage}");
        // Retry loading after a delay
        Invoke(nameof(LoadRewardedAd), 3f);
    }

    private void OnRewardedAdDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.Log($"Rewarded ad displayed. Network: {adInfo.AdNetwork}");
    }

    private void OnRewardedAdDisplayFailed(LevelPlayAdDisplayInfoError error)
    {
        Debug.LogError($"Rewarded ad failed to display: {error.ToString()}");
        // Load a new ad for next time
        LoadRewardedAd();
    }

    private void OnRewardedAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        Debug.Log($"User rewarded! Reward: {reward.Name} - Amount: {reward.Amount}");
        
        // Grant reward to the player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GrantReward();
        }
    }

    private void OnRewardedAdClosed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Rewarded ad closed");
        // Load a new ad for next time
        LoadRewardedAd();
    }

    private void OnRewardedAdClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void OnDestroy()
    {
        // Unregister events to prevent memory leaks
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
        
        if (_rewardedAd != null)
        {
            _rewardedAd.OnAdLoaded -= OnRewardedAdLoaded;
            _rewardedAd.OnAdLoadFailed -= OnRewardedAdLoadFailed;
            _rewardedAd.OnAdDisplayed -= OnRewardedAdDisplayed;
            _rewardedAd.OnAdDisplayFailed -= OnRewardedAdDisplayFailed;
            _rewardedAd.OnAdRewarded -= OnRewardedAdRewarded;
            _rewardedAd.OnAdClosed -= OnRewardedAdClosed;
            _rewardedAd.OnAdClicked -= OnRewardedAdClicked;
        }
    }
} 