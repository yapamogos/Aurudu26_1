using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
     [SerializeField] private Animator animator;

     public enum PlayerState
     {
        Idle,
        Clapping,
        Showing,
        Mic,
        Onchilla,
        KambaAdeema,
        KottaPora,
        LissanaGaha,
        Rabana,
        KanaMutti,
        Aubone

     }
    public PlayerState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public GameObject[] Boyobjects;
    public GameObject[] Girlobjects;


    public int CharacterIndex;
    public int ColorIndex;

    [SerializeField] private Material[] boyMaterials;
    [SerializeField] private Material[] girlMaterials;


    [SerializeField] private GameObject Kotte;
    [SerializeField] private GameObject Polla;
    [SerializeField] private Animator kotteAnimator;


    private GeneralManager generalManager;

    void Start()
    {
        UpdateCharacter();
       Kotte.SetActive(false);
       Polla.SetActive(false);

        if(currentState == PlayerState.Idle)
        {
            animator.SetInteger("Index", 0 );
        }
        else if(currentState == PlayerState.Clapping)
        {
            animator.SetInteger("Index", 1 );
        }
        else if(currentState == PlayerState.Showing)
        {
            animator.SetInteger("Index", 2 );
        }
        else if(currentState == PlayerState.Mic)
        {
            animator.SetInteger("Index", 3 );
        }
        else if(currentState == PlayerState.Onchilla)
        {
            animator.SetInteger("Index", 4 );
        }
        else if(currentState == PlayerState.KambaAdeema)
        {
            animator.SetInteger("Index", 5 );
        }
        else if(currentState == PlayerState.KottaPora)
        {
            animator.SetInteger("Index", 6 );
            Kotte.SetActive(true);
        }
        else if(currentState == PlayerState.LissanaGaha)
        {
            animator.SetInteger("Index", 7 );
        }
         else if(currentState == PlayerState.Rabana)
        {
            animator.SetInteger("Index", 8 );
        }
         else if(currentState == PlayerState.KanaMutti)
        {
            animator.SetInteger("Index", 9 );            
            Polla.SetActive(true);
        }
        else if(currentState == PlayerState.Aubone)
        {
            animator.SetInteger("Index", 10 );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCharacter()
    {
        CharacterIndex = (CharacterIndex + 1) % 2; // Toggle between 0 and 1
        UpdateCharacter();
        generalManager = GeneralManager.Instance;
        generalManager.SetCurrentCharacterIndex(CharacterIndex);
    }

    public void ChangeColor()
    {

        ColorIndex = (ColorIndex + 1) % 4; // Cycle through available colors
        UpdateCharacter();
        generalManager = GeneralManager.Instance;
        generalManager.SetCurrentColorIndex(ColorIndex);
    }

    public void UpdateCharacter()
    {
         if(CharacterIndex == 0)
        {
            foreach(GameObject obj in Boyobjects)
            {
                obj.SetActive(true);
                obj.GetComponent<Renderer>().material = boyMaterials[ColorIndex];
            }
            foreach(GameObject obj in Girlobjects)
            {
                obj.SetActive(false);
            }
        }
        else if(CharacterIndex == 1)
        {
            foreach(GameObject obj in Boyobjects)
            {
                obj.SetActive(false);
            }
            foreach(GameObject obj in Girlobjects)
            {
                obj.SetActive(true);
                obj.GetComponent<Renderer>().material = girlMaterials[ColorIndex];
            }
        }
    }

    public void Run(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }

    public void Climb(bool isClimbing)
    {
        animator.SetBool("isClimbing", isClimbing);
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
        kotteAnimator.SetTrigger("attack");
    }

    public void React()
    {
        animator.SetTrigger("react");
    }

    public void Mutti()
    {
        animator.SetTrigger("Mutti");
    }
}
