using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TugOfWarManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rope;
    [SerializeField] private float winDistance = 5f;
    [SerializeField] private MovingButton movingButton;

    [Header("Settings")]
    [SerializeField] private float pullStrength = 0.5f;
    [SerializeField] private float autoPullSpeed = 1.0f;
    [SerializeField] private float smoothSpeed = 5f;

    private float startZ;
    private float targetZ;
    private bool gameEnded = false;
    private bool gameStarted = false;


    bool startAiPull = false;

    [Header("Game Stats")]
    float gameTime = 0;
    float score = 0;
    bool isTimerOn = false;

    [Header("Score Settings")]
    [SerializeField] private float bestTime = 5f;
    [SerializeField] private float worstTime = 30f;

    [Header("Win/Lose")]
    [SerializeField] GameObject winLosePanel;
    [SerializeField] TextMeshProUGUI winLoseTxt;
    [SerializeField] ParticleSystem ref_psWin;

    [Header("Score Panel")]
    [SerializeField] GameObject ref_scorePanel;
    [SerializeField] TextMeshProUGUI ref_scoreText;

    [SerializeField] private GameUIController gameUIController;

    void Start()
    {
        winLosePanel.SetActive(false);
        ref_scorePanel.SetActive(false);
        fn_startGame();
    }

    void OnEnable()
    {
        GameUIController.OnTimerFinished += startGame;
    }

    void OnDisable()
    {
        GameUIController.OnTimerFinished -= startGame;
    }

    public void fn_startGame()
    {

        // Reset all game values
        gameEnded = false;
        gameStarted = false;
        startAiPull = false;
        gameTime = 0;
        isTimerOn = false;

        startZ = rope.position.z;
        targetZ = startZ;

        rope.position = new Vector3(rope.position.x, rope.position.y, startZ);

    }

    void Update()
    {
        if (gameEnded || !gameStarted) return;

        if (startAiPull)
        {
            targetZ += autoPullSpeed * Time.deltaTime;
        }

        float newZ = Mathf.Lerp(rope.position.z, targetZ, smoothSpeed * Time.deltaTime);
        rope.position = new Vector3(rope.position.x, rope.position.y, newZ);

        CheckWinByOffset();
        StartScoreTimer();
    }

    public void PullPlayer()
    {
        if (!gameEnded && gameStarted)
        {
            targetZ -= pullStrength;
        }
    }

    private void CheckWinByOffset()
    {
        float currentOffset = rope.position.z - startZ;

        if (currentOffset >= winDistance)
        {
            EndGame("You Lose...!");
        }
        else if (currentOffset <= -winDistance)
        {
            EndGame("You Win...!");
            //ref_psWin.Play();
            //CalculateScore();
        }

    }

    private void EndGame(string message)
    {
        gameEnded = true;
        isTimerOn = false;

        if (movingButton != null)
            movingButton.StopMoving();

        winLoseTxt.text = message;
        winLosePanel.SetActive(true);
        gameUIController.GameOver("KambaAdeema");

        if (message == "You Win...!")
        {
            float clampedTime = Mathf.Clamp(gameTime, bestTime, worstTime);
            float t = (clampedTime - bestTime) / (worstTime - bestTime);
            score = Mathf.Round(Mathf.Lerp(100f, 1f, t));

        }
        else
        {
            score = 0; // Lose gives 0 score
            ref_scoreText.text = "0";
            // ref_scorePanel.SetActive(true);
            
        }

        StartCoroutine(ShowScorePanelAfterDelay());
    }


    void startGame()
    {
        // Game officially starts here
        startAiPull = true;
        gameStarted = true;
        isTimerOn = true;

        if (movingButton != null)
        {
            movingButton.gameObject.SetActive(true);
            movingButton.StartMoving();
        }
    }

    void StartScoreTimer()
    {
        if (isTimerOn)
        {
            gameTime += Time.deltaTime;
        }
    }

    IEnumerator ShowScorePanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        gameUIController.ShowGameOverPanel(Mathf.RoundToInt(score), "KambaAdeema");
    }
}
