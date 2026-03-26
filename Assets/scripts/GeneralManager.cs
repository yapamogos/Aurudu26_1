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

    public int wasanaMuttiScore = 0;
    public int KambaAdeemaScore = 0;
    public int KottaPoraScore = 0;
    public int AliyataAhaThabimaScore = 0;
    public int LissanaGahaScore = 0;

    public bool wasanaMuttiPlayed = false;
    public bool KambaAdeemaPlayed = false;
    public bool KottaPoraPlayed = false;
    public bool AliyataAhaThabimaPlayed = false;
    public bool LissanaGahaPlayed = false;


    public Vector3 PlayerLastPosition ;



    public int MyTotalScore
    {
        get { 
            return wasanaMuttiScore + KambaAdeemaScore + KottaPoraScore + AliyataAhaThabimaScore + LissanaGahaScore; 
            }
        set
        {
            
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

        
        PlayerLastPosition = new Vector3(-2.4f, 0, -44.4f);
    }


    void Start()
    {
        wasanaMuttiPlayed = true;
        KambaAdeemaPlayed = true;
        KottaPoraPlayed = true;
        AliyataAhaThabimaPlayed = true;
        LissanaGahaPlayed = true;


 
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

    string uniqueSessionId = Guid.NewGuid().ToString();

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            // Use our custom generator instead of SystemInfo
            CustomId = GetOrCreatePlayerID(), 
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    string GetOrCreatePlayerID()
    {
        // 1. Check if we already have an ID saved in this browser's cache
        string savedID = PlayerPrefs.GetString("Uniq_Player_ID", "");

        if (string.IsNullOrEmpty(savedID))
        {
            // 2. If not, generate a brand new unique one
            // Guid.NewGuid() is statistically impossible to duplicate
            savedID = Guid.NewGuid().ToString();
            
            // 3. Save it so they get the same account next time
            PlayerPrefs.SetString("Uniq_Player_ID", savedID);
            PlayerPrefs.Save();
            Debug.Log("Generated new WebGL ID: " + savedID);
        }

        return savedID;
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        SetDisplayName(PlayerPrefs.GetString("MyName", "Guest"));
    }

    void OnLoginFailure(PlayFabError error)
    {
    Debug.LogError(error.GenerateErrorReport());
    }

    public void SubmitScore(int score) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreSubmitSuccess, OnError);
    }

    void OnScoreSubmitSuccess(UpdatePlayerStatisticsResult result) 
    {
        Debug.Log("Score submitted successfully!");
    }

    void OnError(PlayFabError error) {
        Debug.LogError(error.GenerateErrorReport());
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
        Debug.Log("Successfully updated PlayFab Display Name to: " + result.DisplayName);
    }
}
