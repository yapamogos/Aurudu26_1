using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyMenuManager : MonoBehaviour
{

    public Player player;
    [SerializeField] private TextMeshProUGUI TotalScoreText;
    private GeneralManager generalManager;

    void LateUpdate()
    {
        int totalScore = generalManager.MyTotalScore;
        TotalScoreText.text = totalScore.ToString();
    } 

    void Start()
    {
        generalManager = GeneralManager.Instance;
        if(generalManager.MyTotalScore > 0)
        {
            generalManager.SubmitScore(generalManager.MyTotalScore);   
        }
    }  

    public void StartGame(string sceneName)
    {
        player.SetPlayerPosition();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        
    }

    public void HomeButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}
