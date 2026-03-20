using UnityEngine;

public class GameAreaTriigger : MonoBehaviour
{
    [SerializeField] private GameNameUI myGameNameUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myGameNameUI.FoucusButton();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myGameNameUI.UnFoucusButton();
        }
    }
}
