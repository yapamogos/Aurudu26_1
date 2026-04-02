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


    [SerializeField] private GameObject navMeshObject;

    [SerializeField] private GameObject LeaderBoard;

    [SerializeField] private GameObject LeaderBoardEntryPrefab;



    void LateUpdate()
    {
        float totalScore = generalManager.MyTotalScore;
        TotalScoreText.text = totalScore.ToString("F2");
    } 

    void Awake()
    {
        generalManager = GeneralManager.Instance;
        if(generalManager.MyTotalScore > 0)
        {
            generalManager.SubmitScore(generalManager.MyTotalScore);   
        }

        generalManager.GetLeaderboard();

        navMeshObject.SetActive(false);

        if(generalManager.LastSceneName == "Home")
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
        }

        navMeshObject.SetActive(true);
        LeaderBoard.SetActive(false);
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

    public void ShowLeaderBoard()
    {
        LeaderBoard.SetActive(true);
        for(int i = 0; i < generalManager.leaderboardLines.Count; i++)
        {
            GameObject entry = Instantiate(LeaderBoardEntryPrefab, LeaderBoard.transform.GetChild(1).transform);
            TextMeshProUGUI entryText = entry.GetComponentInChildren<TextMeshProUGUI>();
            entryText.text = generalManager.leaderboardLines[i];
        }
    }

    public void HideLeaderBoard()
    {
        LeaderBoard.SetActive(false);
        for(int i = 0; i < LeaderBoard.transform.GetChild(1).childCount; i++)
        {
            Destroy(LeaderBoard.transform.GetChild(1).GetChild(i).gameObject);
        }
    }
}
