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
        Login();
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

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier, // Unique ID for this device
            //CustomId = uniqueSessionId, // Unique ID for this session
            CreateAccount = true // Creates a new account if one doesn't exist
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
    Debug.Log("Login successful!");
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
