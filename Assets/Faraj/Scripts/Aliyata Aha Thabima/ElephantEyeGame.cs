 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ElephantEyeGame2D : MonoBehaviour
{
    [SerializeField] private GameUIController gameUIController;

    [Header("Game Settings")]
    [SerializeField] private int maxScore = 100;
    [SerializeField] private float countdownTime = 20f; // 20 seconds countdown

    [Header("Boundaries")]
    [SerializeField] private SpriteRenderer movementAreaSprite;
    [SerializeField] private float dotPadding = 0.2f;

    [Header("2D Sprite References")]
    [SerializeField] private SpriteRenderer elephantSprite;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer eyeMarker;

    [Header("Moving Dot Settings")]
    [SerializeField] private SpriteRenderer movingDot;
    [SerializeField] private float minSpeed = 1.5f;
    [SerializeField] private float maxSpeed = 3.5f;
    private Color originalDotColor;

    [Header("UI References")]
    [SerializeField] private SpriteRenderer blackoutPanel;
    [SerializeField] private SpriteRenderer SecondBlackoutPanel;
    [SerializeField] private GameObject instructionPanel; // Panel with instructions shown at start
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText; // Countdown timer
    [SerializeField] private Button exitButton; // Exit to main menu button
    [SerializeField] private string mainMenuSceneName = "Menu"; // Name of main menu scene

    [Header("Eye Position (Local to Sprite)")]
    [SerializeField] private Vector2 eyePosition;

    private bool canTouch = false;
    private bool dotIsMoving = false;
    private Vector3 eyeWorldPosition;
    private Vector3 dotVelocity;    private float currentSpeed;
    private Color originalEyeColor;
    private float remainingTime;
    private bool timerRunning = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        eyeWorldPosition = elephantSprite.transform.TransformPoint(eyePosition);
        blackoutPanel.color = new Color(1, 1, 1, 0f);

        blackoutPanel.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        movingDot.gameObject.SetActive(false);

       

        instructionText.text = "Tap anywhere to start!";
        instructionText.color = Color.black;

        originalDotColor = movingDot.color;
        originalEyeColor = eyeMarker.color;

        ResetDotVelocity();

                // Position dot at random starting position
        Bounds bounds = movementAreaSprite.bounds;
        float startX = Random.Range(bounds.min.x + dotPadding, bounds.max.x - dotPadding);
        float startY = Random.Range(bounds.min.y + dotPadding, bounds.max.y - dotPadding);
        
        movingDot.gameObject.SetActive(true);        
        movingDot.transform.position = new Vector3(startX, startY, -1);
        dotIsMoving = true;
        SecondBlackoutPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canTouch && dotIsMoving)
        {
            StopDotAndShowResults();
        }

        if (dotIsMoving)
        {
            MoveDot();
        }

        // Update countdown timer
        if (timerRunning)
        {
            UpdateTimer();
        }
    }

    private void OnEnable()
    {
        // Subscribe to the timer event
        GameUIController.OnTimerFinished += StartGame;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks/errors
        GameUIController.OnTimerFinished -= StartGame;
    }

    void StartGame()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        instructionText.text = "This is eye point!";
        instructionText.color = Color.yellow;

        eyeMarker.gameObject.SetActive(true);



 
        canTouch = false;

        eyeMarker.gameObject.SetActive(false);

        instructionText.text = "Get Ready...";
        instructionText.color = Color.white;
        movingDot.sortingOrder = -1;

        yield return StartCoroutine(SlideBlackoutDown());

        instructionText.text = "TAP to stop the dot!";
        instructionText.color = Color.green;
        canTouch = true;

        // Start countdown timer
        StartTimer();
    }

    void MoveDot()
    {
        Vector3 nextPos = movingDot.transform.position + dotVelocity * Time.deltaTime;
        Bounds bounds = movementAreaSprite.bounds;

        bool bounced = false;

        // Bounce on LEFT / RIGHT walls
        if (nextPos.x <= bounds.min.x + dotPadding)
        {
            nextPos.x = bounds.min.x + dotPadding;
            dotVelocity.x = Mathf.Abs(dotVelocity.x);
            bounced = true;
        }
        else if (nextPos.x >= bounds.max.x - dotPadding)
        {
            nextPos.x = bounds.max.x - dotPadding;
            dotVelocity.x = -Mathf.Abs(dotVelocity.x);
            bounced = true;
        }

        // Bounce on TOP / BOTTOM walls
        if (nextPos.y <= bounds.min.y + dotPadding)
        {
            nextPos.y = bounds.min.y + dotPadding;
            dotVelocity.y = Mathf.Abs(dotVelocity.y);
            bounced = true;
        }
        else if (nextPos.y >= bounds.max.y - dotPadding)
        {
            nextPos.y = bounds.max.y - dotPadding;
            dotVelocity.y = -Mathf.Abs(dotVelocity.y);
            bounced = true;
        }

        // Change speed on bounce
        if (bounced)
        {
            currentSpeed = Random.Range(minSpeed, maxSpeed);
            Vector3 direction = dotVelocity.normalized;
            dotVelocity = direction * currentSpeed;
        }

        movingDot.transform.position = nextPos;
    }

    void ResetDotVelocity()
    {
        Vector2 dir;
        do
        {
            dir = Random.insideUnitCircle.normalized;
        }
        while (Mathf.Abs(dir.x) < 0.3f || Mathf.Abs(dir.y) < 0.3f);

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        dotVelocity = new Vector3(dir.x, dir.y, 0) * currentSpeed;
    }

    void StartTimer()
    {
        remainingTime = countdownTime;
        timerRunning = true;
    }

    void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;


        // Time's up - auto stop
        if (remainingTime <= 0f)
        {
            timerRunning = false;
            StopDotAndShowResults();
        }
    }

    IEnumerator SlideBlackoutDown()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += 2f * Time.deltaTime;
            blackoutPanel.color = new Color(1, 1, 1, alpha);
            yield return null;
        }   

        SecondBlackoutPanel.gameObject.SetActive(true);
    }

    IEnumerator SlideBlackoutUp()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= 2f * Time.deltaTime;
            blackoutPanel.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }

    void StopDotAndShowResults()
    {
        dotIsMoving = false;
        canTouch = false;
        timerRunning = false;


        Vector3 finalDotPosition = movingDot.transform.position;
        float distance = Vector2.Distance(finalDotPosition, eyeWorldPosition);
        float score = CalculateScore(distance);
        StartCoroutine(ShowResultsWithAnimation(score, distance, finalDotPosition));

        gameUIController.GameOver("AliyataAhaThabima");
    }

    IEnumerator ShowResultsWithAnimation(float score, float distance, Vector3 dotPosition)
    {
        SecondBlackoutPanel.gameObject.SetActive(false);
        yield return StartCoroutine(SlideBlackoutUp());

        movingDot.sortingOrder = 5;

        instructionText.text = "Let's see how you did...";
        instructionText.color = Color.cyan;

        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(0.5f);

        eyeMarker.gameObject.SetActive(true);
        Color c = eyeMarker.color;
        c.a = 0.4f;
        eyeMarker.color = c;

        CreateDottedLine(dotPosition, eyeWorldPosition);

        //scoreText.gameObject.SetActive(true);
        scoreText.text = $"Distance: {distance:F2} units\n{GetScoreMessage(score)}";

        instructionText.text = GetResultMessage(score);
        //instructionText.color = GetResultColor(score);

        yield return new WaitForSeconds(2f);
        gameUIController.ShowGameOverPanel(score, "AliyataAhaThabima");

    }

    float CalculateScore(float distance)
    {
        float maxDistance = 2.5f;
        distance = Mathf.Clamp(distance, 0f, maxDistance);
        float normalized = 1f - (distance / maxDistance);
        float score = normalized * maxScore;
        score = Mathf.Max(score, 10);
        return score;
    }

    void CreateDottedLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("DistanceLine");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = Color.white;
        lineRenderer.material = lineMaterial;

        float distance = Vector3.Distance(start, end);
        int numDots = Mathf.Max(10, Mathf.RoundToInt(distance * 10));
        float dotSpacing = 0.1f;

        lineRenderer.positionCount = numDots;

        Vector3 startPos = new Vector3(start.x, start.y, -2);
        Vector3 endPos = new Vector3(end.x, end.y, -2);
        Vector3 direction = (endPos - startPos).normalized;
        float totalDistance = Vector3.Distance(startPos, endPos);

        int dotIndex = 0;
        for (float d = 0; d < totalDistance && dotIndex < numDots; d += dotSpacing * 2)
        {
            Vector3 dotPosition = startPos + direction * d;
            lineRenderer.SetPosition(dotIndex, dotPosition);

            dotIndex++;
            if (dotIndex < numDots)
            {
                float nextD = Mathf.Min(d + dotSpacing * 0.5f, totalDistance);
                lineRenderer.SetPosition(dotIndex, startPos + direction * nextD);
                dotIndex++;
            }
        }

        Vector3 lastPos = lineRenderer.GetPosition(dotIndex - 1);
        for (int i = dotIndex; i < numDots; i++)
        {
            lineRenderer.SetPosition(i, lastPos);
        }

        lineRenderer.sortingOrder = 5;
    }

    string GetScoreMessage(float score)
    {
        if (score == maxScore)
            return "PERFECT! Bull's Eye!";
        else if (score >= maxScore * 0.8f)
            return "Excellent!";
        else if (score >= maxScore * 0.6f)
            return "Good job!";
        else if (score >= maxScore * 0.4f)
            return "Not bad!";
        else if (score >= maxScore * 0.2f)
            return "Keep trying!";
        else
            return "Better luck next time!";
    }

    string GetResultMessage(float score)
    {
        if (score == maxScore)
            return "AMAZING! Perfect shot!";
        else if (score >= maxScore * 0.8f)
            return "Great timing!";
        else if (score >= maxScore * 0.6f)
            return "Nice try!";
        else
            return "Try again!";
    }

    /*Color GetResultColor(int score)
    {
        if (score >= maxScore * 0.8f)
            return Color.green;
        else if (score >= maxScore * 0.5f)
            return Color.yellow;
        else
            return new Color(1f, 0.5f, 0f);
    }*/

    // Called by Exit button
    public void ExitToMainMenu()
    {
        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDrawGizmos()
    {
        if (movementAreaSprite != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(movementAreaSprite.bounds.center, movementAreaSprite.bounds.size);
        }
    }
}