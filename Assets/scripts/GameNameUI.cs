using UnityEngine;

public class GameNameUI : MonoBehaviour
{
    [SerializeField] private GameObject GameButton;

    public void FoucusButton()
    {
        GameButton.GetComponent<Animator>().SetTrigger("Focus");
    }

    public void UnFoucusButton()
    {
        GameButton.GetComponent<Animator>().SetTrigger("UnFocus");
    }
}
