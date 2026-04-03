using UnityEngine;

public class kambaAdeemaPlayer : MonoBehaviour
{

    private PlayerAnimator playerAnimator;
    private GeneralManager generalManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
