using UnityEngine;
using UnityEngine.UI; 

public class MuteBtn : MonoBehaviour
{

    private GeneralManager generalManager;

    [SerializeField] private Sprite MuteIcon;
    [SerializeField] private Sprite UnMuteIcon;

    private Image spriteRenderer;


    private void Awake()
    {
        
        spriteRenderer = GetComponent<Image>();
    }

    void Start()
    {
        Invoke(nameof(SetIcon), 0.5f); // Delay to ensure GeneralManager is initialized
    }

    void SetIcon()
    {
        generalManager = GeneralManager.Instance;
        if (generalManager.isBackgroundMusicPlaying)
        {
            spriteRenderer.sprite = UnMuteIcon;
        }
        else
        {
            spriteRenderer.sprite = MuteIcon;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


  
    public void ToggleMute()
    {
        generalManager = GeneralManager.Instance;
        generalManager.setBackgroundMusicPlaying();
        if (generalManager.isBackgroundMusicPlaying)
        {
            spriteRenderer.sprite = UnMuteIcon;
        }
        else
        {
            spriteRenderer.sprite = MuteIcon;
        }
    }
}
