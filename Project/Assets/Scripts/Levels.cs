using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    // public variables
    public List<GameObject> aliens = new List<GameObject>(); // List of alien game objects in the level
    public List<GameObject> player = new List<GameObject>(); // List of player game objects in the level
    public GameObject remy, claire; // References to player avatars: Remy and Claire
    
    // Called before the first frame update
    void Start()
    {
        // Check the player's preference (using PlayerPrefs) to determine which avatar to activate
        // Default value is 0 if no value is found

        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Check the player's avatar selection (0 for Remy, 1 for Claire)
            if (PlayerPrefs.GetInt("Avatar", 0) == 0)
            {
                // Activate Remy and deactivate Claire if the player selected Remy
                remy.SetActive(true);
                claire.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
            {
                // Activate Claire and deactivate Remy if the player selected Claire
                remy.SetActive(false);
                claire.SetActive(true);
            }
        }
    }
    
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None; // Locks the cursor
        Cursor.visible = true;
    }
}