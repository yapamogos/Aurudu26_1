using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class GameUIController : MonoBehaviour
{
    public bool ShowScoreText = true;
    public bool ShowRoundText = true;
    public bool ShowTimeText = true;

    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private GameObject _ScorePanel;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private TextMeshProUGUI _timeText;


    public float countdownTime = 3f; 

    public float GameTime = 0f;

    public bool isPlaying = false;
    // This is the "Messenger"
    public static event Action OnTimerFinished;

    private int _score;
    public int Score 
    {   
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreText.text = "Score: " + value.ToString();
        }
    }

    private int _round;
    public int Round
    {
        get
        {
            return _round;
        }

        set
        {
            _roundText.text = "Round: " + value.ToString();
        }
    }

    public string GameOverMessage
    {
        set
        {
            _gameOverText.text = value;
        }
    }


    private GeneralManager generalManager;

     void Start()
    {
        _gameOverPanel.SetActive(false);
        _ScorePanel.SetActive(false);
        _timerPanel.SetActive(true);
        StartCoroutine(StartCountdown());
        isPlaying = false;
        generalManager = GeneralManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            GameTime += Time.deltaTime;
            _timeText.text = "Time: " + GameTime.ToString("F2");
        }
    }

    IEnumerator StartCountdown()
    {
        _timerPanel.SetActive(true);
        float time = countdownTime;

        while (time > 0)
        {
            _timerText.text = Mathf.Ceil(time).ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }

        _timerText.text = "GO!";
        yield return new WaitForSeconds(1f);

        _timerPanel.SetActive(false);

        OnTimerFinished?.Invoke();
        isPlaying = true;
        _scoreText.gameObject.SetActive(ShowScoreText);
        _roundText.gameObject.SetActive(ShowRoundText);
        _timeText.gameObject.SetActive(ShowTimeText);
        if(ShowRoundText || ShowScoreText || ShowTimeText)
            _ScorePanel.SetActive(true);    
    }

    public void GameOver(string gameName)
    {
        isPlaying = false;
        if(gameName == "WasanaMutti")
        {
            generalManager.wasanaMuttiPlayed = false;
        }
        else if(gameName == "KambaAdeema")
        {
            generalManager.KambaAdeemaPlayed = false;
        }
        else if(gameName == "KottaPora")
        {
            generalManager.KottaPoraPlayed = false;
        }
        else if(gameName == "AliyataAhaThabima")
        {
            generalManager.AliyataAhaThabimaPlayed = false;
        }
        else if(gameName == "LissanaGaha")
        {
            generalManager.LissanaGahaPlayed = false;
        }
    }



    public void ShowGameOverPanel(int finalScore , string gameName )
    {
        generalManager.SubmitScore(finalScore);
        GameOverMessage = $"{finalScore}";
        _gameOverPanel.SetActive(true);

        if(gameName == "WasanaMutti")
        {
            generalManager.wasanaMuttiScore = finalScore;
        }
        else if(gameName == "KambaAdeema")
        {
            generalManager.KambaAdeemaScore = finalScore;
        }
        else if(gameName == "KottaPora")
        {
            generalManager.KottaPoraScore = finalScore;
        }
        else if(gameName == "AliyataAhaThabima")
        {
            generalManager.AliyataAhaThabimaScore = finalScore;
        }
        else if(gameName == "LissanaGaha")
        {
            generalManager.LissanaGahaScore = finalScore;
        }

        
    }

    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
