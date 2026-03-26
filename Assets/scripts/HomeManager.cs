using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject NamePanel;
    [SerializeField] private GameObject StartButton;

    [SerializeField] private TMP_InputField nameInputField;

    private GeneralManager generalManager;

    void Start()
    {
        if (PlayerPrefs.HasKey("MyName") && PlayerPrefs.HasKey("Uniq_Player_ID"))
        {
            NamePanel.SetActive(false);
            StartButton.SetActive(true);
        }
        else
        {
            NamePanel.SetActive(true);
            StartButton.SetActive(false);
        }

        generalManager = GeneralManager.Instance;
    }

    public void SetName()
    {
        string name = nameInputField.text;
        PlayerPrefs.SetString("MyName", name);
        PlayerPrefs.Save();
        NamePanel.SetActive(false);
        StartButton.SetActive(true);
    }



    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        generalManager.Login();
    }

    public void Menutransition()
    {
        this.GetComponent<Animator>().Play("HomeTrans");
    }
}
