using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Kottaporamanager : MonoBehaviour
{
    [Header("Start Panel")]

    [Header("UI References")]
    [SerializeField] private Slider timingSlider;
    [SerializeField] private BoxCollider2D sliderCollider;    // moving collider (prefer Handle)
    [SerializeField] private BoxCollider2D targetCollider;
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private RectTransform sliderTrackRect;
    [SerializeField] private Button hitButton;

    [Header("Texts (TMP)")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text shotText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text roundTimerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private TMP_Text scoreText1;

    [Header("Animators")]
    //[SerializeField] private Animator playerAnimator;
    //[SerializeField] private Animator aiAnimator;

    [SerializeField] private PlayerAnimator myPlayerAnimator;
    [SerializeField] private PlayerAnimator AIPlayerAnimator;


    [Header("Gameplay Settings")]
    [SerializeField] private int totalRounds = 3;
    [SerializeField] private int shotsPerRound = 3;
    [SerializeField] private float shotTimeLimit = 2.5f;
    [SerializeField] private float resolveLockTime = 0.35f;

    [Header("Slider Speed Per Shot (Editable)")]
    public float sliderSpeedNormal = 1.0f;
    public float sliderSpeedMedium = 1.6f;
    public float sliderSpeedFast = 2.2f;

    [Header("Target Size (px)")]
    [SerializeField] private float targetHeightRound1 = 80f;
    [SerializeField] private float targetHeightMin = 18f;

    [Header("Target Position Range")]
    [SerializeField] private float targetMinNormalized = 0.15f;
    [SerializeField] private float targetMaxNormalized = 0.85f;

    public int roundIndex;
    public int shotIndex;
    public int shotRound;
    private int hitsThisRound;

    private float timeLeftThisShot;
    private float totalScore;
    private float pointsPerHit;

    private bool goingUp;
    private bool resolved;
    private bool gameStarted;

    private GeneralManager generalManager;

    [SerializeField] private GameUIController gameUIController;

    private void Start()
    {
        if (!sliderTrackRect)
            sliderTrackRect = timingSlider.GetComponent<RectTransform>();

        // Slider setup
        timingSlider.interactable = false;
        timingSlider.minValue = 0f;
        timingSlider.maxValue = 1f;
        timingSlider.wholeNumbers = false;

        // Buttons
        hitButton.onClick.RemoveListener(OnHitPressed);
        hitButton.onClick.AddListener(OnHitPressed);

        // Score
        pointsPerHit = 100f / (totalRounds * shotsPerRound);

        // Initial UI state
        
        // Start the game immediately for testing; replace with instruction panel logic if needed
        hitButton.interactable = false;
        gameStarted = false;
        resolved = true; // block Update loop until game starts

        totalScore = 0f;
        if (resultText) resultText.text = "";
        shotRound = 1;

        UpdateUI();
        Innitialize();



        

        generalManager = GeneralManager.Instance;
        if(generalManager == null)
        {
            Debug.LogError("GeneralManager instance not found! Please ensure it exists in the scene.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
        myPlayerAnimator.CharacterIndex = generalManager.currentCharacterIndex;
        myPlayerAnimator.ColorIndex = generalManager.currentColorIndex;
        myPlayerAnimator.UpdateCharacter();
    }


    void OnEnable()
    {
        GameUIController.OnTimerFinished += StartGame;
    }

    void OnDisable()
    {
        GameUIController.OnTimerFinished -= StartGame;
    }

    private void Update()
    {
        if (!gameStarted || resolved) return;

        // Timer
        timeLeftThisShot -= Time.deltaTime;
        UpdateTimerUI();

        if (timeLeftThisShot <= 0f)
        {
            ResolveShot(false, "MISS!");
            return;
        }

        // Slider movement
        float speed = GetCurrentSpeed();
        float value = timingSlider.value;
        float delta = speed * Time.deltaTime;

        value += goingUp ? delta : -delta;

        if (value >= 1f) { value = 1f; goingUp = false; }
        if (value <= 0f) { value = 0f; goingUp = true; }

        timingSlider.value = value;
    }

    private void Innitialize()
    {
        // Lock gameplay during countdown
        gameStarted = false;
        resolved = true;
        hitButton.interactable = false;
        gameUIController.TotalRounds = 9;
        gameUIController.Round = 1;
        gameUIController.Score = 0;

    }

    public void StartGame()
    {
              // Start game
        gameStarted = true;
        StartNewRound(1);
        UpdateUI();
    }

    private void OnHitPressed()
    {
        if (!gameStarted || resolved) return;

        bool success = sliderCollider && targetCollider && sliderCollider.IsTouching(targetCollider);
        ResolveShot(success, success ? "HIT!" : "MISS!");
    }

    private float GetCurrentSpeed()
    {
        if (shotIndex == 1) return sliderSpeedNormal;
        if (shotIndex == 2) return sliderSpeedMedium;
        return sliderSpeedFast;
    }

    private void ResolveShot(bool success, string message)
    {
        resolved = true;
        hitButton.interactable = false;

        if (resultText) resultText.text = message;

        if (success)
        {
            myPlayerAnimator.Attack();
            AIPlayerAnimator.React();
            hitsThisRound++;
            totalScore += pointsPerHit;
            gameUIController.Score = totalScore;
        }
        else
        {
            myPlayerAnimator.React();
            AIPlayerAnimator.Attack();
        }

        totalScore = Mathf.Clamp(totalScore, 0f, 100f);
        UpdateScoreUI();

        Invoke(nameof(AdvanceAfterShot), resolveLockTime);
    }

    private void AdvanceAfterShot()
    {

        shotIndex++;
        if(shotRound < gameUIController.TotalRounds)
        {
            shotRound++;
        }
        gameUIController.Round = shotRound;
        if (shotIndex <= shotsPerRound)
        {
            StartNewShot();
            return;
        }



        Invoke(nameof(AdvanceAfterRound), 0.6f);
    }

    private void AdvanceAfterRound()
    {
        
        if (roundIndex >= totalRounds)
        {
            EndGame();
            return;
        }

        roundIndex++;
        StartNewRound(roundIndex);
        UpdateUI();
    }

    private void StartNewRound(int round)
    {
        roundIndex = round;
        shotIndex = 1;
        hitsThisRound = 0;

        // Target gets smaller each round
        float t = (round - 1f) / (totalRounds - 1f);
        float height = Mathf.Lerp(targetHeightRound1, targetHeightMin, t);

        if (targetCollider)
        {
            Vector2 s = targetCollider.size;
            s.y = height;
            targetCollider.size = s;
        }

        if (targetRect)
        {
            Vector2 vis = targetRect.sizeDelta;
            vis.y = height;
            targetRect.sizeDelta = vis;
        }

        RandomizeTargetPosition();
        StartNewShot();
    }

    private void StartNewShot()
    {
        resolved = false;
        hitButton.interactable = true;

        timeLeftThisShot = shotTimeLimit;

        timingSlider.value = Random.Range(0f, 1f);
        goingUp = Random.value > 0.5f;

        if (resultText) resultText.text = "";
        UpdateUI();
    }

    private void RandomizeTargetPosition()
    {
        if (!targetRect || !sliderTrackRect) return;

        float norm = Random.Range(targetMinNormalized, targetMaxNormalized);
        float h = sliderTrackRect.rect.height;
        float y = Mathf.Lerp(-h / 2f, h / 2f, norm);

        Vector2 pos = targetRect.anchoredPosition;
        pos.y = y;
        targetRect.anchoredPosition = pos;
    }

    private void EndGame()
    {
        hitButton.interactable = false;
        resolved = true;

        float finalScore = totalScore;

        if (roundText) roundText.text = "GAME OVER";
        if (shotText) shotText.text = "";
        if (roundTimerText) roundTimerText.text = "0.0s";
        if (resultText) resultText.text = $"FINAL SCORE: {finalScore}/100";
        //WinPanel.SetActive(true);
        if(scoreText1) scoreText1.text = $"FINAL SCORE: {finalScore}/100";

        UpdateScoreUI();

        gameUIController.GameOver("KottaPora");
        gameUIController.ShowGameOverPanel(finalScore, "KottaPora");
    }

    // ---------- UI ----------
    private void UpdateUI()
    {
        if (!gameStarted)
        {
            if (roundText) roundText.text = $"Get Ready!";
            if (shotText) shotText.text = "";
            if (roundTimerText) roundTimerText.text = "";
            UpdateScoreUI();
            return;
        }

        if (shotText) shotText.text = $"Shot {shotIndex}/{shotsPerRound}  Hits: {hitsThisRound}";

        UpdateTimerUI();
        UpdateScoreUI();
    }

    private void UpdateTimerUI()
    {
        if (!roundTimerText) return;
        roundTimerText.text = $"{Mathf.Max(0f, timeLeftThisShot):0.0}s";
    }

    private void UpdateScoreUI()
    {
        if (!scoreText) return;
        scoreText.text = $"Score: {Mathf.RoundToInt(totalScore)}/100";
        gameUIController.Score = totalScore;
    }
}
