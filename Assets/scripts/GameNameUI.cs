using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameNameUI : MonoBehaviour
{
    [SerializeField] private GameObject GameButton;
    private GeneralManager generalManager;

    [SerializeField] private CameraFollow cameraFollow;

    public enum GameNames
    {
        WasanaMutti,
        KambaAdeema,
        KottaPora,
        AliyataAhaThabima,
        LissanaGaha
    }

     public GameNames gameName;

    void Start()
    {
        generalManager = GeneralManager.Instance;
        if(gameName == GameNames.WasanaMutti && generalManager.WasanaMuttiHas ||
           gameName == GameNames.KambaAdeema && generalManager.KambaAdeemaHas ||
           gameName == GameNames.KottaPora && generalManager.KottaPoraHas ||
           gameName == GameNames.AliyataAhaThabima && generalManager.AliyataAhaThabimaHas ||
           gameName == GameNames.LissanaGaha && generalManager.LissanaGahaHas)
        {
            GameButton.SetActive(true);
        }
        else
        {
            GameButton.SetActive(false);
        }
    }

    void OnEnable()
    {
        CameraFollow.OnMapViewActivated += mapSize;
        CameraFollow.OnMapViewDeactivated += normalSize;
    }
    void OnDisable()
    {
        CameraFollow.OnMapViewActivated -= mapSize;
        CameraFollow.OnMapViewDeactivated -= normalSize;

    }

    public void FoucusButton()
    {
        GameButton.GetComponent<Animator>().SetBool("Focus",true);
    }

    public void UnFoucusButton()
    {
        GameButton.GetComponent<Animator>().SetBool("Focus",false);
    }

    public void mapSize()
    {
        GameButton.GetComponent<Animator>().SetBool("ms", true);
    }

    public void normalSize()
    {
        GameButton.GetComponent<Animator>().SetBool("ms", false);
    }

    
}
