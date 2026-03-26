using UnityEngine;

public class GameAreaTriigger : MonoBehaviour
{
    [SerializeField] private GameNameUI myGameNameUI;
    
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
