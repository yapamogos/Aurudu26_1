using UnityEngine;

public class Costumer : MonoBehaviour
{
    public GameObject[] Boyobjects;
    public GameObject[] Girlobjects;

    public int CharacterIndex;
    public int ColorIndex;

    [SerializeField] private Material[] boyMaterials;
    [SerializeField] private Material[] girlMaterials;

    public void Start()
    {
        CharacterIndex = Random.Range(0, 2);
        ColorIndex = Random.Range(0, 4);
        UpdateCharacter();
    }


    public void UpdateCharacter()
    {
         if(CharacterIndex == 0)
        {
            foreach(GameObject obj in Boyobjects)
            {
                obj.SetActive(true);
                obj.GetComponent<Renderer>().material = boyMaterials[ColorIndex];
            }
            foreach(GameObject obj in Girlobjects)
            {
                obj.SetActive(false);
            }
        }
        else if(CharacterIndex == 1)
        {
            foreach(GameObject obj in Boyobjects)
            {
                obj.SetActive(false);
            }
            foreach(GameObject obj in Girlobjects)
            {
                obj.SetActive(true);
                obj.GetComponent<Renderer>().material = girlMaterials[ColorIndex];
            }
        }
    }
}
