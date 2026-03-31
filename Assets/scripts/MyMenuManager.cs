using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyMenuManager : MonoBehaviour
{

    public Player player;
    [SerializeField] private TextMeshProUGUI TotalScoreText;
    private GeneralManager generalManager;

    [SerializeField] private GameObject StartLoc;

    [SerializeField] private GameObject wasanaMuttiLoc;
    [SerializeField] private GameObject KambaAdeemaLoc;
    [SerializeField] private GameObject KottaPoraLoc;   
    [SerializeField] private GameObject AliyataAhaThabimaLoc;
    [SerializeField] private GameObject LissanaGahaLoc;



    void LateUpdate()
    {
        int totalScore = generalManager.MyTotalScore;
        TotalScoreText.text = totalScore.ToString();
    } 

    void Awake()
    {
        generalManager = GeneralManager.Instance;
        if(generalManager.MyTotalScore > 0)
        {
            generalManager.SubmitScore(generalManager.MyTotalScore);   
        }


       /* if(generalManager.LastSceneName == "Home")
        {
            player.transform.position = StartLoc.transform.position;
        }
        else if(generalManager.LastSceneName == "KanaMutti")
        {
            player.transform.position = wasanaMuttiLoc.transform.position;
        }
        else if(generalManager.LastSceneName == "KambaAdeema")
        {
            player.transform.position = KambaAdeemaLoc.transform.position;
        }
        else if(generalManager.LastSceneName == "Kottapora")
        {
            player.transform.position = KottaPoraLoc.transform.position;
        }
        else if(generalManager.LastSceneName == "Aliya")
        {
            player.transform.position = AliyataAhaThabimaLoc.transform.position;
        }
        else if(generalManager.LastSceneName == "LissanaGaha")
        {
            player.transform.position = LissanaGahaLoc.transform.position;
        }
        else
        {
            player.transform.position = StartLoc.transform.position;
        }*/
    }  

    public void StartGame(string sceneName)
    {
        player.SetPlayerPosition();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        generalManager.LastSceneName = sceneName;
        
    }

    public void HomeButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        generalManager.LastSceneName = "Home";
    }
}
