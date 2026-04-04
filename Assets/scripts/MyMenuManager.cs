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
    [SerializeField] private GameObject LeaderBoardContent;

    [SerializeField] private GameObject LeaderBoardEntryPrefab;


    [SerializeField] private Sprite PlayedSprite;
    [SerializeField] private Sprite NotPlayedSprite;

    [SerializeField] private Image wasanaMuttiLocImage;
    [SerializeField] private Image KambaAdeemaImage;
    [SerializeField] private Image KottaporaImage;
    [SerializeField] private Image AliyaImage;
    [SerializeField] private Image LissanaGahaImage;



    [SerializeField] private GameObject CongtratsPanel;
    [SerializeField] private GameObject NumberInput;
    [SerializeField] private GameObject SaveButton;
    [SerializeField] private GameObject conExitButton;
    [SerializeField] private GameObject conReplayButton;
    [SerializeField] private TextMeshProUGUI ConTotalScoreText;




    void LateUpdate()
    {
        float totalScore = generalManager.MyTotalScore;
        TotalScoreText.text = totalScore.ToString("F2");
    } 
    

    void OnEnable()
    {
        GeneralManager.OnLeaderboardUpdated += ShowPlayersInLeaderBoard;
    }

    void OnDisable()
    {
        GeneralManager.OnLeaderboardUpdated -= ShowPlayersInLeaderBoard;
    }
    void Awake()
    {
        generalManager = GeneralManager.Instance;
        if(generalManager.MyTotalScore > 0)
        {
            generalManager.SubmitScore(generalManager.MyTotalScore);   
        }

        //generalManager.GetLeaderboard();

        navMeshObject.SetActive(false);
        player.transform.gameObject.SetActive(false);
        player.canMove = false;

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

        
        LeaderBoard.SetActive(false);
        LeaderBoardContent.SetActive(false);
        CongtratsPanel.SetActive(false);
        if(!generalManager.WasanaMuttiHas && !generalManager.KambaAdeemaHas && !generalManager.KottaPoraHas && !generalManager.AliyataAhaThabimaHas && !generalManager.LissanaGahaHas)
        {
            CongratsPanel();
        }


        wasanaMuttiLocImage.sprite = generalManager.WasanaMuttiHas ? NotPlayedSprite : PlayedSprite;
        KambaAdeemaImage.sprite = generalManager.KambaAdeemaHas ? NotPlayedSprite : PlayedSprite;
        KottaporaImage.sprite = generalManager.KottaPoraHas ? NotPlayedSprite : PlayedSprite;
        AliyaImage.sprite = generalManager.AliyataAhaThabimaHas ? NotPlayedSprite : PlayedSprite;
        LissanaGahaImage.sprite = generalManager.LissanaGahaHas ? NotPlayedSprite : PlayedSprite;

        navMeshObject.SetActive(true);
        player.transform.gameObject.SetActive(true);
        player.canMove = true;

    }  


    public void StartGame(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        generalManager.LastSceneName = sceneName;
        
    }

    public void HomeButton()
    {
        generalManager.LastSceneName = "Home";
        generalManager.SetRePlay();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        
    }

    public void ShowLeaderBoard()
    {
        generalManager.GetLeaderboard();
        LeaderBoard.SetActive(true);
        
    }


    public void ShowPlayersInLeaderBoard()
    {
        LeaderBoardContent.SetActive(true);
        for(int i = 0; i < LeaderBoardContent.transform.childCount; i++)
        {
            Destroy(LeaderBoardContent.transform.GetChild(i).gameObject);
        }
        for(int i = 0; i < generalManager.leaderboardLines.Count; i++)
        {
            GameObject entry = Instantiate(LeaderBoardEntryPrefab, LeaderBoardContent.transform);
            TextMeshProUGUI entryText = entry.GetComponentInChildren<TextMeshProUGUI>();
            entryText.text = generalManager.leaderboardLines[i];
        }
    }

    public void HideLeaderBoard()
    {
        LeaderBoard.SetActive(false);
        LeaderBoardContent.SetActive(false);
        for(int i = 0; i < LeaderBoardContent.transform.childCount; i++)
        {
            Destroy(LeaderBoardContent.transform.GetChild(i).gameObject);
        }
    }

    public void CongratsPanel()
    {
        CongtratsPanel.SetActive(true);
        if (PlayerPrefs.HasKey("PhoneNumber") && !string.IsNullOrEmpty(PlayerPrefs.GetString("PhoneNumber")))
        {
            NumberInput.SetActive(false);
            SaveButton.SetActive(false);
            conExitButton.SetActive(true);
            conReplayButton.SetActive(true);
        }
         else
        {
            NumberInput.SetActive(true);
            SaveButton.SetActive(true);
            conExitButton.SetActive(false);
            conReplayButton.SetActive(false);
            SaveButton.GetComponent<Button>().interactable = false;
        }

        float totalScore = generalManager.MyTotalScore;
        TotalScoreText.text = totalScore.ToString("F2");
        ConTotalScoreText.text = totalScore.ToString("F2");

        
    }

    public void SaveAndSetNumber()
    {
        string phoneNumber = NumberInput.GetComponent<TMP_InputField>().text;
        PlayerPrefs.SetString("PhoneNumber", phoneNumber);
        PlayerPrefs.Save();
        generalManager.SaveCustomPlayerData(phoneNumber);
        NumberInput.SetActive(false);
        SaveButton.SetActive(false);
        conExitButton.SetActive(true);
        conReplayButton.SetActive(true);
    }

    public void checkValidInput()
    {
        if (!string.IsNullOrEmpty(NumberInput.GetComponent<TMP_InputField>().text) )
        {
            if ((NumberInput.GetComponent<TMP_InputField>().text.Length == 10 && long.TryParse(NumberInput.GetComponent<TMP_InputField>().text, out _)) || string.IsNullOrEmpty(NumberInput.GetComponent<TMP_InputField>().text))
            {
                SaveButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                SaveButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            SaveButton.GetComponent<Button>().interactable = false;
        }
    }

}
