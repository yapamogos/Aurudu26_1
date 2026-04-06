using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        /*bool newplayer = false;
        string name = nameInputField.text;
        string OldName = PlayerPrefs.GetString("MyName", "");
        if (name != OldName)
        {
            newplayer = true;
        }
        PlayerPrefs.SetString("MyName", name);
        string phoneNumber = numberInputField.text;
        PlayerPrefs.SetString("PhoneNumber", phoneNumber);
        PlayerPrefs.Save();
        NamePanel.SetActive(false);
        StartButton.SetActive(true);        
        generalManager.Login(newplayer);*/
        
        StartCoroutine(DelaySetName());
    }

    public IEnumerator DelaySetName()
    {
        bool newplayer = false;
        string name = nameInputField.text;
        string OldName = PlayerPrefs.GetString("MyName", "");
        if (name != OldName)
        {
            newplayer = true;
        }
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        PlayerPrefs.SetString("MyName", name);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        string phoneNumber = numberInputField.text;
        PlayerPrefs.SetString("PhoneNumber", phoneNumber);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        generalManager.Login(newplayer);
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        NamePanel.SetActive(false);
        StartButton.SetActive(true);        
        
    }

    /*public void LoggedIn()
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
    */
    public void checkValidInput()
    {
        // 1. Extract the values to make the logic cleaner
        string name = nameInputField.text;
        string number = numberInputField.text;

        // 2. Define your individual validation rules
        bool isNameValid = !string.IsNullOrEmpty(name) && name.Length >= 3;

        // Number is valid if it's empty (optional) OR (it's exactly 10 digits and numeric)
        bool isNumberValid = string.IsNullOrEmpty(number) || 
                            (number.Length == 10 && long.TryParse(number, out _));

        // 3. The button is only interactable if BOTH are valid
        LoginButton.interactable = isNameValid && isNumberValid;
    }



    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        generalManager.SetDisplayName(PlayerPrefs.GetString("MyName", "Guest"));
        generalManager.SaveCustomPlayerData(PlayerPrefs.GetString("PhoneNumber", "NoNumber"));
    }

    public void Menutransition()
    {
        this.GetComponent<Animator>().Play("HomeTrans");
    }

    
}
