using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    // public variables
    // List of alien game objects in the level
    public List<GameObject> aliens = new List<GameObject>();

    // List of player game objects in the level
    public List<GameObject> player = new List<GameObject>();

    // Singleton instance of the Levels class
    public static Levels instance;

    // References to player avatars: Remy and Claire
    public GameObject remy, claire;
    
    // Called when the script instance is being loaded
    void Awake()
    {
        // Assigns the instance of this script to the static Instance variable (Singleton pattern)
        instance = this;
    }

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
    


    
    
        // This method is called when the GameObject is enabled.
        // It unlocks the mouse cursor by setting the cursor lock state to None.
        // private void OnEnable()
        // {
        //     Cursor.lockState = CursorLockMode.Locked; // Unlock the cursor, allowing free movement outside the game window.
        //     Cursor.visible = false;
        // }

        // This method is called when the GameObject is disabled.
        // It locks the mouse cursor by setting the cursor lock state to Locked.
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None; // Lock the cursor to the center of the game window.
            Cursor.visible = true;
        }

        
}
