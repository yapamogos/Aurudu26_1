using UnityEngine;
using System.Collections;
using System.Threading;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager Instance { get; private set; }

    [Header("Frame Settings")]
    int MaxRate = 120;
    public float TargetFrameRate = 120.0f;
    float currentFrameTime;

    public float wasanaMuttiScore = 0;
    public float KambaAdeemaScore = 0;
    public float KottaPoraScore = 0;
    public float AliyataAhaThabimaScore = 0;
    public float LissanaGahaScore = 0;

    public bool WasanaMuttiHas = false;
    public bool KambaAdeemaHas = false;
    public bool KottaPoraHas = false;
    public bool AliyataAhaThabimaHas = false;
    public bool LissanaGahaHas = false;


    public Vector3 PlayerLastPosition ;

    public string LastSceneName;

    [Header("Audio Settings")]
    public AudioSource bgmSource; // Assign this in the Inspector
    private Coroutine volumeCoroutine;

    public float MyTotalScore
    {
        get { 
            return wasanaMuttiScore + KambaAdeemaScore + KottaPoraScore + AliyataAhaThabimaScore + LissanaGahaScore; 
            }
        set
        {
            
        }
    }

    public static event Action OnLeaderboardUpdated;

    public List<string> leaderboardLines = new List<string>();

    public bool isBackgroundMusicPlaying
    {
        get { return bgmSource != null && bgmSource.isPlaying; }
        set {
            if (bgmSource == null) return;

            if (value && !bgmSource.isPlaying)
            {
                PlayMusicWithFade(0.8f, 3.0f); // Start music with fade-in
            }
            else if (!value && bgmSource.isPlaying)
            {
                bgmSource.Stop(); // Stop music immediately
                if (volumeCoroutine != null) StopCoroutine(volumeCoroutine); // Stop any ongoing fade
            }
        }
    }





    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MaxRate;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine("WaitForNextFrame");

        
        PlayerLastPosition = new Vector3(-2.4f, 0, -55f);
    }


    void Start()
    {
        WasanaMuttiHas = true;
        KambaAdeemaHas = true;
        KottaPoraHas = true;
        AliyataAhaThabimaHas = true;
        LissanaGahaHas = true;

        LastSceneName = "Home";


        wasanaMuttiScore = 0;
        KambaAdeemaScore = 0;   
        KottaPoraScore = 0;
        AliyataAhaThabimaScore = 0; 
        LissanaGahaScore = 0;

        bool isMusicPlaying = PlayerPrefs.GetInt("IsMusicPlaying", 1) == 1; // Default to true (1)
        isBackgroundMusicPlaying = isMusicPlaying; // This will handle starting/stopping music based on saved preference
        Debug.Log("Background music is " + (isMusicPlaying ? "enabled" : "disabled") + " based on saved preference.");
        if (isMusicPlaying)
        {
            PlayMusicWithFade(0.8f, 3.0f);
        }
    }


    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / TargetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }

    public int currentCharacterIndex = 0;
    public int currentColorIndex = 0;

    public void SetCurrentCharacterIndex(int index)
    {
        currentCharacterIndex = index;
    }

    public void SetCurrentColorIndex(int index)
    {
        currentColorIndex = index;
    }

    public void PlayMusicWithFade(float targetVolume = 1.0f, float duration = 2.0f)
    {
        if (bgmSource == null)
        {
            //Debug.LogWarning("BGM Source not assigned on GeneralManager!");
            return;
        }

        // Stop any existing fade to prevent conflicts
        if (volumeCoroutine != null) StopCoroutine(volumeCoroutine);

        // Start playing at 0 volume
        bgmSource.volume = 0;
        bgmSource.Play();

        // Start the smooth increase
        volumeCoroutine = StartCoroutine(FadeVolume(targetVolume, duration));
    }

    private IEnumerator FadeVolume(float targetVolume, float duration)
    {
        float currentTime = 0;
        float startVolume = bgmSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // Move volume linearly from start to target over time
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    string uniqueSessionId = Guid.NewGuid().ToString();

    public void Login(bool newplayer)
    {
        var request = new LoginWithCustomIDRequest
        {
            // Use our custom generator instead of SystemInfo
            CustomId = GetOrCreatePlayerID(newplayer), 
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    string GetOrCreatePlayerID(bool newPlayer)
    {
        // 1. Check if we already have an ID saved in this browser's cache
        string savedID = PlayerPrefs.GetString("Uniq_Player_ID", "");

        if (string.IsNullOrEmpty(savedID) || newPlayer)
        {
            // 2. If not, generate a brand new unique one
            // Guid.NewGuid() is statistically impossible to duplicate
            savedID = Guid.NewGuid().ToString();
            
            // 3. Save it so they get the same account next time
            PlayerPrefs.SetString("Uniq_Player_ID", savedID);
            PlayerPrefs.Save();
            //Debug.Log("Generated new WebGL ID: " + savedID);
        }

        return savedID;
    }

    void OnLoginSuccess(LoginResult result)
    {
        //Debug.Log("Login successful!");
        SetDisplayName(PlayerPrefs.GetString("MyName", "Guest"));
    }

    void OnLoginFailure(PlayFabError error)
    {
    //Debug.LogError(error.GenerateErrorReport());
    }

    /*public void SubmitScore(float score) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "HighScore",
                    Value = (int)(score*100) // Convert to int by multiplying by 100
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreSubmitSuccess, OnError);
    }*/

    public void SubmitScore(float score, string ghyt)
    {
        int highScore = (int)(score * 100);
        string savedID = PlayerPrefs.GetString("Uniq_Player_ID", "");

        if(ghyt != savedID)
        {
            //Debug.LogWarning("Score submission failed: Player name mismatch.");
            return; // Exit early if the player name doesn't match
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = highScore
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            // ✅ Stat updated successfully — now log event for PlayStream
            var eventRequest = new WriteClientPlayerEventRequest
            {
                EventName = "player_highscore_updated",
                Body = new Dictionary<string, object>
                {
                    { "HighScore", highScore }
                }
            };

            PlayFabClientAPI.WritePlayerEvent(eventRequest,
                eventResult =>
                {
                    //UnityEngine.Debug.Log($"HighScore submitted & logged: {highScore}");
                },
                error =>
                {
                    //Debug.LogError("Failed to write PlayStream event: " + error.GenerateErrorReport());
                });

        },
        error =>
        {
            //Debug.LogError("Failed to update player statistics: " + error.GenerateErrorReport());
        });
    }

    void OnScoreSubmitSuccess(UpdatePlayerStatisticsResult result) 
    {
        //Debug.Log("Score submitted successfully!");
    }


    public void SetDisplayName(string nameToSet)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameToSet
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        //Debug.Log("Successfully updated PlayFab Display Name to: " + result.DisplayName);
        SaveCustomPlayerData(PlayerPrefs.GetString("PhoneNumber", "NoNumber"));
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore", // Must match the name in SubmitScore
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardListSuccess, OnError);
    }

    void OnGetLeaderboardListSuccess(GetLeaderboardResult result)
    {
        leaderboardLines.Clear(); // Clear old data

        foreach (var item in result.Leaderboard)
        {
            // Convert the int back to a float (matching your SubmitScore logic)
            float actualScore = item.StatValue / 100.0f;
            
            // Format: "#1 | Name | 500.25"


                string entry = string.Format("{0,-3} | {1,-20} | {2,10}", 
                item.Position + 1, 
                (item.DisplayName ?? "Player"), 
                actualScore.ToString("F2"));

            leaderboardLines.Add(entry);
        }

        OnLeaderboardUpdated?.Invoke(); // Notify listeners that the leaderboard has been updated

        //Debug.Log("Leaderboard list updated with " + leaderboardLines.Count + " entries.");
    }


    public void SetRePlay()
    {
        wasanaMuttiScore = 0;
        KambaAdeemaScore = 0;
        KottaPoraScore = 0;
        AliyataAhaThabimaScore = 0;
        LissanaGahaScore = 0;


        WasanaMuttiHas = true;
        KambaAdeemaHas = true;
        KottaPoraHas = true;
        AliyataAhaThabimaHas = true;
        LissanaGahaHas = true;

        Loggout();
    }


    public void Loggout()
    {
        PlayFabSettings.staticPlayer.ClientSessionTicket = null;
    }
         // Reset the specific game data based on the gameName


    /*public void SaveCustomPlayerData(string number)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Number", number },
            },
            // Set to Public so other players can see it, or Private (default)
            Permission = UserDataPermission.Private 
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSaved, OnError);
    }*/

    public void SaveCustomPlayerData(string number)
    {
        // 1. Save to Player Data (same as before)
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Number", number },
            },
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("Player data saved!");

            // 2. SEND EVENT FOR ANALYTICS (🔥 THIS IS THE KEY)
            SendPlayerNumberEvent(number);

        }, OnError);
    }

    void SendPlayerNumberEvent(string number)
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "player_number_updated",
            Body = new Dictionary<string, object>
            {
                { "Number", number }
            }
        };

        PlayFabClientAPI.WritePlayerEvent(request,
            result => Debug.Log("Player number event sent!"),
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnDataSaved(UpdateUserDataResult result)
    {
        //Debug.Log("Successfully saved custom player data!");
    }

    void OnError(PlayFabError error) {
    // Check if the error is specifically about the Display Name being taken
        if (error.Error == PlayFabErrorCode.NameNotAvailable || error.ErrorMessage.Contains("Name not available"))
        {
            //Debug.LogWarning("The chosen display name is already taken. Try a different one!");
            // Logic to handle the duplicate name (e.g., append a random number or show a UI popup)
            HandleDuplicateName(); 
        }
        else
        {
            // Standard error logging for everything else
            //Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
        }
    }

    void HandleDuplicateName()
    {
        // Example: Try setting the name again with a random suffix
        string currentName = PlayerPrefs.GetString("MyName", "Guest");
        string newName = currentName + UnityEngine.Random.Range(100, 999).ToString();

        PlayerPrefs.SetString("MyName", newName);
        PlayerPrefs.Save();
        
        //Debug.Log("Retrying with: " + newName);
        SetDisplayName(newName);
    }

    public void setBackgroundMusicPlaying()
    {
        isBackgroundMusicPlaying = !isBackgroundMusicPlaying;
        PlayerPrefs.SetInt("IsMusicPlaying", isBackgroundMusicPlaying ? 1 : 0);
        PlayerPrefs.Save();
    }


}
