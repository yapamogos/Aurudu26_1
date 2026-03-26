using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameNameUI : MonoBehaviour
{
    [SerializeField] private GameObject GameButton;
    private GeneralManager generalManager;

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
        if(gameName == GameNames.WasanaMutti && generalManager.wasanaMuttiPlayed ||
           gameName == GameNames.KambaAdeema && generalManager.KambaAdeemaPlayed ||
           gameName == GameNames.KottaPora && generalManager.KottaPoraPlayed ||
           gameName == GameNames.AliyataAhaThabima && generalManager.AliyataAhaThabimaPlayed ||
           gameName == GameNames.LissanaGaha && generalManager.LissanaGahaPlayed)
        {
            GameButton.SetActive(true);
        }
        else
        {
            GameButton.SetActive(false);
        }
    }

    public void FoucusButton()
    {
        GameButton.GetComponent<Animator>().SetTrigger("Focus");
    }

    public void UnFoucusButton()
    {
        GameButton.GetComponent<Animator>().SetTrigger("UnFocus");
    }

    
}
