using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private GameObject NamePanel;
    [SerializeField] private GameObject StartButton;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField numberInputField;

    [SerializeField] private Button LoginButton;

    private GeneralManager generalManager;

    void Start()
    {
        /*if (PlayerPrefs.HasKey("MyName") && PlayerPrefs.HasKey("Uniq_Player_ID"))
        {
            NamePanel.SetActive(false);
            StartButton.SetActive(true);
        }
        else
        {
            NamePanel.SetActive(true);
            StartButton.SetActive(false);
        }*/

        NamePanel.SetActive(true);
        StartButton.SetActive(false);

        if (PlayerPrefs.HasKey("MyName"))
        {
            nameInputField.text = PlayerPrefs.GetString("MyName");
            LoginButton.interactable = true;

        }

        if (PlayerPrefs.HasKey("PhoneNumber"))
        {
            numberInputField.text = PlayerPrefs.GetString("PhoneNumber");
        }

        generalManager = GeneralManager.Instance;
    }

    public void SetName()
    {
        string name = nameInputField.text;
        PlayerPrefs.SetString("MyName", name);
        string phoneNumber = numberInputField.text;
        PlayerPrefs.SetString("PhoneNumber", phoneNumber);
        PlayerPrefs.Save();
        NamePanel.SetActive(false);
        StartButton.SetActive(true);        
        generalManager.Login();
    }

    public void LoggedIn()
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
    }
    
    public void checkValidInput()
    {
        if (!string.IsNullOrEmpty(nameInputField.text) )
        {
            if ((numberInputField.text.Length == 10 && long.TryParse(numberInputField.text, out _)) || string.IsNullOrEmpty(numberInputField.text))
            {
                LoginButton.interactable = true;
            }
            else
            {
                LoginButton.interactable = false;
            }
        }
        else
        {
            LoginButton.interactable = false;
        }
    }



    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void Menutransition()
    {
        this.GetComponent<Animator>().Play("HomeTrans");
    }
}
