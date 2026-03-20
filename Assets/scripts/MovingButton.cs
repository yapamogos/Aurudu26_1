using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovingButton : MonoBehaviour
{
    [Header("Button Locations")]
    [SerializeField] private Transform[] buttonPositions; // Array of 4 positions

    [Header("Movement Settings")]
    [SerializeField] private float stayDuration = 3f; // How long button stays in one place
    [SerializeField] private float moveSpeed = 10f; // Speed of movement transition

    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private int lastPositionIndex = -1;
    private bool isMoving = false;
    private bool gameStarted = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Start at position 01 (index 0)
        if (buttonPositions.Length > 0)
        {
            lastPositionIndex = 0;
            targetPosition = buttonPositions[0].GetComponent<RectTransform>().anchoredPosition;
            rectTransform.anchoredPosition = targetPosition;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Smoothly move to target position
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Check if we've reached the target
            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 0.1f)
            {
                rectTransform.anchoredPosition = targetPosition;
                isMoving = false;
            }
        }
    }

    // Call this when the game starts (after countdown)
    public void StartMoving()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            StartCoroutine(MoveRoutine());
        }
    }

    // Call this when game ends
    public void StopMoving()
    {
        gameStarted = false;
        StopAllCoroutines();
    }

    private IEnumerator MoveRoutine()
    {
        while (gameStarted)
        {
            // Stay at current position for stayDuration seconds
            yield return new WaitForSeconds(stayDuration);

            // Move to new random position
            MoveToRandomPosition(false);

            // Wait for movement to complete
            while (isMoving)
            {
                yield return null;
            }
        }
    }

    private void MoveToRandomPosition(bool instant = false)
    {
        if (buttonPositions.Length == 0)
        {
            Debug.LogError("No button positions assigned!");
            return;
        }

        // Get a random position that's different from the last one
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, buttonPositions.Length);
        } while (randomIndex == lastPositionIndex && buttonPositions.Length > 1);

        lastPositionIndex = randomIndex;
        targetPosition = buttonPositions[randomIndex].GetComponent<RectTransform>().anchoredPosition;

        if (instant)
        {
            rectTransform.anchoredPosition = targetPosition;
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
}