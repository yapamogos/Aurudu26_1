using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LissanaPlayer : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    public float slideSpeed = 3f;
    public float slideSpeedlow = 1f;
    public float slideSpeedHigh = 3f;

    public float energyConsumptionRate = 10f;
    public float energyRecoveryRate = 5f;



    private float highestPointofslowsliding = 0f;

    private float climbEnergyMultiplier = 1f;
    private float SlideEnergyMultiplier = 1f;

    private float checkTimer ;
    private float checkTimerReset = 5f;
    public KeyCode climbKey = KeyCode.Space;

    public float Energy = 100f;
    public bool isClimbing = false;

    [SerializeField] private Slider energyBar;
    [SerializeField] private GameObject LissanaGaha;

     public float[] initialPosition;
     public float height;

     private Rigidbody rb;

     private bool isPlaying = true;
    [SerializeField] private TextMeshProUGUI TimeText;

     private float finishTime;
     private float Timetmp;

    private PlayerAnimator playerAnimator;
    private GeneralManager generalManager;

    [SerializeField] private GameUIController gameUIController;

    void Start()
    {
        Timetmp = 0f;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Disable physics interactions


        slideSpeed = slideSpeedHigh;

        if (energyBar != null)
        {
            energyBar.value = Energy/100f;
        }

        initialPosition = new float[10];
        float totalHeight = LissanaGaha.GetComponent<Renderer>().bounds.size.y; // The global height if parent isn't scaled
        Vector3 worldPos = LissanaGaha.transform.position;

        height = totalHeight;

        for (int i = 0; i < initialPosition.Length; i++)
        {
            // 1. Start at the center (worldPos.y)
            // 2. Subtract half the height to get to the bottom
            // 3. Add the increment based on the index
            initialPosition[i] = (worldPos.y - totalHeight / 2f) + (i * (totalHeight / 10f));
        }
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

        isPlaying = false;
    }
    void OnEnable()
    {
        GameUIController.OnTimerFinished += startGame;
    }

    void OnDisable()
    {
        GameUIController.OnTimerFinished -= startGame;
    }


    public void startGame()
    {
        isPlaying = true;
    }

    void Update()
    {
        
        UpdateHighestPointLogic(transform.position.y); 

        if(Energy > 95f)
        {
            climbEnergyMultiplier = 2f;
            SlideEnergyMultiplier = 0.8f;
        }        
        else if(Energy >60f)
        {
            climbEnergyMultiplier = 1.2f;
            SlideEnergyMultiplier = 0.9f;
        }
        else if(Energy >10f)
        {
            climbEnergyMultiplier = 0.6f;
            SlideEnergyMultiplier = 1f;
        }
        else if(Energy >1f)
        {
            climbEnergyMultiplier = 0.3f;
            SlideEnergyMultiplier = 3f;
        }
        else
        {
            climbEnergyMultiplier = 0.15f;
            SlideEnergyMultiplier = 5f;
        }

        float heightWeight = 1f;

        if(transform.position.y >= initialPosition[8])
        {
            heightWeight = 2f;
        }
        else if(transform.position.y >= initialPosition[7])
        {
            heightWeight = 1.8f;
        }
        else if(transform.position.y >= initialPosition[6])
        {
            heightWeight = 1.6f;
        }
        else if(transform.position.y >= initialPosition[5])
        {
            heightWeight = 1.4f;
        }
        else if(transform.position.y >= initialPosition[4])
        {
            heightWeight = 1.2f;
        }

        if(transform.position.y >= initialPosition[0] && transform.position.y <= initialPosition[9])
        {

            rb.isKinematic = true; // Enable kinematic to disable physics interactions

            if ((Input.GetKey(climbKey) || isClimbing )&& isPlaying)
            {
                ClimbUp();
                if(Energy > 0 && isPlaying)
                {
                    Energy -= Time.deltaTime * energyConsumptionRate * heightWeight; // Decrease energy while climbing
                }
            }
            else
            {
                if(isPlaying)
                {
                    SlideDown();
                }
                
                if(Energy < 100f)
                {
                    Energy += Time.deltaTime * energyRecoveryRate; // Increase energy while sliding down
                }
            }
        }
        else if(transform.position.y > initialPosition[9])
        {
            SlideDown();
        }
        else
        {
            
            if ((Input.GetKey(climbKey) || isClimbing)  && isPlaying)
            {
                ClimbUp();
                if(Energy > 0)
                {
                    Energy -= Time.deltaTime * energyConsumptionRate * heightWeight; // Decrease energy while climbing
                }
                rb.isKinematic = false; // Disable kinematic to enable physics interactions
            }
            else
            {
                rb.isKinematic = true; // Enable kinematic to disable physics interactions

            }

            if(Energy < 100f)
            {
                Energy += Time.deltaTime * energyRecoveryRate; // Increase energy while sliding down
            }
        }

        if(isPlaying)
        {
            Timetmp += Time.deltaTime;
            TimeText.text = Timetmp.ToString("F2");
            finishTime = Timetmp;
        }
        
    }

    void LateUpdate()
    {
        // Update the energy bar UI
        if (energyBar != null)
        {
            float normalizedEnergy = Energy / 100f;
            energyBar.value = normalizedEnergy;
            energyBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, normalizedEnergy);

        }
    }

    void UpdateHighestPointLogic(float currentY)
    {
        // 1. Update the timer
        checkTimer += Time.deltaTime;

        // 2. Every 5 seconds, update the highestPointofslowsliding based on current segment
        if (checkTimer >= checkTimerReset)
        {
            for (int i = 0; i < initialPosition.Length; i++)
            {
                // If player is higher than this specific array point
                if (currentY >= initialPosition[i])
                {
                    highestPointofslowsliding = initialPosition[i];
                    
                }

                
            }
            checkTimer = 0f; // Reset timer
            Debug.Log("Updated checkpoint to: " + highestPointofslowsliding);
        }

        if (currentY >= initialPosition[8])
        {
            playerAnimator.Climb(false);
            if(isPlaying)
            {
                isPlaying = false;
                gameUIController.GameOver("LissanaGaha");
                int finalScore = Mathf.Max(0, 120 - (int)(finishTime * 2f));
                gameUIController.ShowGameOverPanel(finalScore, "LissanaGaha");
            }
        }

        // 3. Logic check: Is the player currently above the recorded checkpoint?
        if (currentY > highestPointofslowsliding)
        {
            slideSpeed = slideSpeedlow; // Normal slide speed
        }
        else
        {
            slideSpeed = slideSpeedHigh; // Increased slide speed
        }
    }

    void ClimbUp()
    {
        // Moves the object upward relative to world space
        transform.Translate(Vector3.up * climbSpeed * climbEnergyMultiplier * Time.deltaTime, Space.World);
        playerAnimator.Climb(true);
    }

    void SlideDown()
    {
        // Moves the object downward relative to world space
        transform.Translate(Vector3.down * slideSpeed * SlideEnergyMultiplier * Time.deltaTime, Space.World);
        playerAnimator.Climb(false);
    }

    public void SetClimbingState(bool climbing)
    {
        isClimbing = climbing;
    }
}