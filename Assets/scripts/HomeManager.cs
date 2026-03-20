using UnityEngine;

public class HomeManager : MonoBehaviour
{

    [SerializeField] private GameObject portraitCanvas;
    [SerializeField] private GameObject landscapeCanvas;
    private void Start()
    {
        if (Screen.width > Screen.height)
        {
            portraitCanvas.SetActive(false);
            landscapeCanvas.SetActive(true);
        }
        else
        {
            portraitCanvas.SetActive(true);
            landscapeCanvas.SetActive(false);
        }
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
