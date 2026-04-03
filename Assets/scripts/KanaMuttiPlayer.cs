using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class KanaMuttiPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float speedIncreasePerRound = 2f;
    [SerializeField] private float minX = -3f;
    [SerializeField] private float maxX = 3f;

    [Header("Game Settings")]
    [SerializeField] private int totalRounds = 3;
    [SerializeField] private float delayBeforeNextRound = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private UnityEngine.UI.Button actionButton;

    [Header("Pots")]
    [SerializeField] private PotSet[] pots = new PotSet[3];

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem correctHitParticles;
    [SerializeField] private ParticleSystem wrongHitParticles;


    [Header("Scoring")]
    [SerializeField] private int wrongPotScore_R12 = 20;
    [SerializeField] private int correctPotScore_R12 = 30;
    [SerializeField] private int wrongPotScore_R3 = 20;
    [SerializeField] private int correctPotScore_R3 = 30;
    [SerializeField] private int perfectScore = 100;

    // Game state
    private float currentSpeed;
    private int currentRound = 1;
    private int direction = 1;
    private bool isMoving = false; // Changed to false initially
    private bool hasChecked = false;
    private bool gameStarted = false; // New flag to track if game has started

    // Pot tracking
    private int currentPotNumber = 0;
    private int brokenPotThisRound = 0;
    private int targetPotNumber = 0;

    // Score tracking
    private float totalScore = 0;
    private int correctHits = 0;

    // Track if current hit is correct (for sound playing)
    private bool isCurrentHitCorrect = false;

    [SerializeField] private GameUIController gameUIController;
    private PlayerAnimator playerAnimator;
    private GeneralManager generalManager;

    [System.Serializable]
    public struct PotSet
    {
        public GameObject full;
        public GameObject broken;
        public Transform potPosition;
    }

    void Start()
    {
        Initialize();
        gameUIController.TotalRounds = totalRounds; // Set total rounds in GameUIController
        gameUIController.Round = currentRound; // Initialize round in GameUIController
        gameUIController.Score = totalScore; // Initialize score in GameUIController


        playerAnimator = GetComponent<PlayerAnimator>();
        if (playerAnimator == null)
        {
            Debug.LogError("PlayerAnimator component not found! Please add it to the player.");
        }

        generalManager = GeneralManager.Instance;
        if(generalManager == null)
        {
            Debug.LogError("GeneralManager instance not found! Please ensure it exists in the scene.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
        playerAnimator.CharacterIndex = generalManager.currentCharacterIndex;
        playerAnimator.ColorIndex = generalManager.currentColorIndex;
        playerAnimator.UpdateCharacter();
        
    }

    void OnEnable()
    {
        GameUIController.OnTimerFinished += GameStart;
    }

    void OnDisable()
    {
        GameUIController.OnTimerFinished -= GameStart;
    }

    void Initialize()
    {
        currentSpeed = moveSpeed;
        ChooseRandomTargetPot();
        SetupAllPots();
        resultText.text = "";


        // Disable action button until game starts
        if (actionButton != null)
        {
            actionButton.interactable = false;
        }
    }

    public void GameStart()
    {
        gameStarted = true;
        isMoving = true;

        if (actionButton != null)
        {
            actionButton.interactable = true;
        }

        Debug.Log("Game Started!");
    }

    void Update()
    {
        if (isMoving && gameStarted)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        transform.position += Vector3.right * (direction * currentSpeed * Time.deltaTime);

        if (transform.position.x >= maxX) direction = -1;
        else if (transform.position.x <= minX) direction = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        currentPotNumber = other.CompareTag("col1") ? 1 :
                          other.CompareTag("col2") ? 2 :
                          other.CompareTag("col3") ? 3 : 0;

        if (currentPotNumber > 0)
            Debug.Log($"Entered Pot {currentPotNumber} zone");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("col1") || other.CompareTag("col2") || other.CompareTag("col3"))
        {
            currentPotNumber = 0;
            Debug.Log("Exited collision zone");
        }
    }

    void ChooseRandomTargetPot()
    {
        targetPotNumber = Random.Range(1, 4);
        Debug.Log($"Round {currentRound} - Target Pot: {targetPotNumber}");
    }

    void SetupAllPots()
    {
        for (int i = 0; i < pots.Length; i++)
        {
            SetPotState(i, true, false);
        }
    }

    void SetPotState(int index, bool showFull, bool showBroken)
    {
        if (index < 0 || index >= pots.Length) return;

        if (pots[index].full) pots[index].full.SetActive(showFull);
        if (pots[index].broken) pots[index].broken.SetActive(showBroken);
    }

    public void OnActionButtonPressed()
    {
        if (hasChecked || !gameStarted) return;

        hasChecked = true;
        isMoving = false;

        if (currentPotNumber > 0)
        {
            HandlePotHit();
        }
        else
        {
            //HandleMiss();
            HandleSoftMiss();
        }
    }

    void HandlePotHit()
    {
        brokenPotThisRound = currentPotNumber;

        // Calculate and add score
        int points = CalculateScore();
        totalScore += points;
        gameUIController.Score = totalScore; // Update score in GameUIController


        // Play animation (Animation Event will call OnHitAnimationEvent)
        playerAnimator.Mutti();

        // Schedule next round after delay
        Invoke(nameof(NextRound), delayBeforeNextRound);
    }

    // THIS METHOD IS CALLED BY ANIMATION EVENT
    public void OnHitAnimationEvent()
    {
        Debug.Log("Animation Event: OnHitAnimationEvent called!");
        BreakPot();
    }

    int CalculateScore()
    {
        bool isCorrect = currentPotNumber == targetPotNumber;
        isCurrentHitCorrect = isCorrect; // Store for sound playing
        int points;

        if (currentRound == 3)
        {
            points = isCorrect ? correctPotScore_R3 : wrongPotScore_R3;
            resultText.text = isCorrect ? $"PERFECT! +{points} points!" : $"Hit! +{points} points!";
            resultText.color = isCorrect ? Color.green : Color.black;
        }
        else
        {
            points = isCorrect ? correctPotScore_R12 : wrongPotScore_R12;
            resultText.text = isCorrect ? $"Perfect! +{points} points!" : $"Hit! +{points} points!";
            resultText.color = isCorrect ? Color.green : Color.black;
        }

        if (isCorrect) correctHits++;

        Debug.Log($"Round {currentRound} - Points: {points} | Total: {totalScore}");
        return points;
    }

    void BreakPot()
    {
        if (brokenPotThisRound == 0) return;

        int index = brokenPotThisRound - 1;
        SetPotState(index, false, true);

        // Play appropriate particles and sound at pot position
        PlayParticlesAndSoundAtPot(index);

        Debug.Log($"Pot {brokenPotThisRound} broken");
    }

    void PlayParticlesAndSoundAtPot(int potIndex)
    {
        if (potIndex < 0 || potIndex >= pots.Length) return;

        // Get pot position
        Vector3 potPosition = pots[potIndex].potPosition != null
            ? pots[potIndex].potPosition.position
            : pots[potIndex].full != null
            ? pots[potIndex].full.transform.position
            : Vector3.zero;

        // Play the appropriate particle system and sound
        if (isCurrentHitCorrect)
        {
            // CORRECT HIT
            if (correctHitParticles != null)
            {
                correctHitParticles.transform.position = potPosition;
                correctHitParticles.Play();
                Debug.Log("Playing CORRECT hit particles");
            }

        }
        else
        {
            // WRONG HIT
            if (wrongHitParticles != null)
            {
                wrongHitParticles.transform.position = potPosition;
                wrongHitParticles.Play();
                Debug.Log("Playing WRONG hit particles");
            }


        }
    }

    void RestorePot()
    {
        if (brokenPotThisRound == 0) return;

        int index = brokenPotThisRound - 1;
        SetPotState(index, true, false);
        Debug.Log($"Pot {brokenPotThisRound} restored");
    }

    void HandleMiss()
    {
        resultText.text = "Missed! Game Over!";
        resultText.color = Color.red;
        actionButton.interactable = false;
        Invoke(nameof(ShowFinalScore), 1f);
    }

    void HandleSoftMiss()
    {
        resultText.text = "Missed!";
        resultText.color = Color.red;
        Invoke(nameof(NextRound), delayBeforeNextRound);
    }

    void NextRound()
    {
        RestorePot();
        currentRound++;
        gameUIController.Round = currentRound; // Update round in GameUIController

        if (currentRound > totalRounds)
        {
            ShowFinalScore();
            
            return;
        }

        // Setup next round
        ChooseRandomTargetPot();
        currentSpeed += speedIncreasePerRound;

        // Reset state
        hasChecked = false;
        isMoving = true;
        currentPotNumber = 0;
        brokenPotThisRound = 0;
        isCurrentHitCorrect = false;
        resultText.text = "";

        // Reset position
        Vector3 pos = transform.position;
        pos.x = minX;
        transform.position = pos;
        direction = 1;
    }

    void  ShowFinalScore()
    {
        if (correctHits == 3)
        {
            totalScore = perfectScore;
            resultText.text = $" PERFECT! HIGH SCORE: {perfectScore}!";
            resultText.color = Color.black;
        }
        else
        {
            resultText.text = $"Game Complete! Final Score: {totalScore}";
            resultText.color = Color.black;
        }
            gameUIController.GameOver("WasanaMutti");
            gameUIController.ShowGameOverPanel(totalScore, "WasanaMutti");
        actionButton.interactable = false;

    }


    public void fn_goBackToMain()
    {
        SceneManager.LoadScene("Menu");
    }
}