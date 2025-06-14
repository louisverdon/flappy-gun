using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsManager Instance { get; private set; }

    [SerializeField] private string _androidGameId = "5876877";
    [SerializeField] private string _iosGameId = "5876876";
    [SerializeField] private bool _testMode = true;

    private string _gameId;
    private const string _androidAdUnitId = "Rewarded_Android";
    private const string _iOSAdUnitId = "Rewarded_iOS";
    private string _adUnitId;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iosGameId;
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _adUnitId = _androidAdUnitId;
#elif UNITY_EDITOR
        _gameId = _androidGameId; // Use Android for editor testing
        _adUnitId = _androidAdUnitId;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void LoadRewardedAd()
    {
        Debug.Log($"Loading Rewarded Ad for placement: {_adUnitId}");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowRewardedAd()
    {
        Debug.Log($"Showing Rewarded Ad for placement: {_adUnitId}");
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad loaded for placement: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed. Granting reward.");
            // Here you would grant the reward to the player.
            // For example, give them extra lives, currency, etc.
            GameManager.Instance.GrantReward(); 
        }
    }
} 