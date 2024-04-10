using UnityEngine;

public class OpenInventory : MonoBehaviour
{
    public GameObject background;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            background.SetActive(!background.activeSelf);
        }
    }
        
        
    
}
