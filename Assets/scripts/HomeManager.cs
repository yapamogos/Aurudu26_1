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
    }

    public void checkValidInput()
    {
        if (!string.IsNullOrEmpty(nameInputField.text) )
        {
            LoginButton.interactable = true;
        }
        else
        {
            LoginButton.interactable = false;
        }
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
