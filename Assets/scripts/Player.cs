using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private float rotationSpeed = 10f; // Controls how fast the player turns

    private GameObject activeIndicator;
    private NavMeshAgent agent;
    private PlayerAnimator playerAnimator;
    private GeneralManager generalManager;

    [SerializeField] private CameraFollow cameraFollow;

    public bool canMove = false; // Flag to control player movement

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found! Please add it to the player.");
        }

        // Optional: Set rotation speed on the agent
        agent.angularSpeed = rotationSpeed * 100f; // NavMesh uses degrees per second

        if (indicatorPrefab != null)
        {
            activeIndicator = Instantiate(indicatorPrefab);
            activeIndicator.SetActive(false);
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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        // Update animation based on agent velocity
        UpdateAnimation();
    }

    private void HandleClick()
    {
        if (EventSystem.current.IsPointerOverGameObject() || cameraFollow.currentState == CameraFollow.CameraState.MapView)
        {
            return; // Exit the function early; don't move the character
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("floor"))
            {
                // Set the destination for the NavMeshAgent
                agent.SetDestination(hit.point);

                if (activeIndicator != null)
                {
                    activeIndicator.transform.position = hit.point + new Vector3(0, 0.1f, 0);
                    activeIndicator.SetActive(true);
                }
            }
        }
    }

    private void UpdateAnimation()
    {
        if(!canMove)
        {
            return;
        }
        // Check if the agent is moving (velocity magnitude > threshold)
        bool isMoving = agent.velocity.magnitude > 0.1f;
        playerAnimator.Run(isMoving);

        // Hide indicator when destination is reached
        if (activeIndicator != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                activeIndicator.SetActive(false);
            }
        }
    }




}