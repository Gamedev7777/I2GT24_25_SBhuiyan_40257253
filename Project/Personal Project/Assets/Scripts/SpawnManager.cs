using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    // public variables
    public static SpawnManager Instance; // Singleton instance of SpawnManager for easy access
    public List<GameObject> alienList = new List<GameObject>(); // List to keep track of all spawned aliens
    public GameObject _levelSpawned;
    public List<GameObject> levelPrefabList = new List<GameObject>();
    
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Sets up the singleton instance
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _levelSpawned = Instantiate(levelPrefabList[PlayerPrefs.GetInt("Level", 1) - 1], new Vector3(0, 0, 0), Quaternion.identity);
        // Spawns a number of aliens based on the current level stored in PlayerPrefs
        for (int i = 0; i < _levelSpawned.GetComponent<Levels>().aliens.Count; i++)
        {
            // Adds the newly spawned alien to the alien list
            alienList.Add(_levelSpawned.GetComponent<Levels>().aliens[i]);
        }
    }

    // Called when the level is completed
    public void LevelComplete()
    {
        // Checks if all aliens have been eliminated
        if (alienList.Count <= 0)
        {
            // If the player has completed all 7 levels, shows the final pop-up and reset the level
            if (PlayerPrefs.GetInt("Level", 1) == 7)
            {
                GameManager.Instance.popUpList[7].SetActive(true); // Shows the final level completion pop-up
                PlayerPrefs.SetInt("Level", 1); // Resets level to 1
            }
            else
            {
                // Increments the level and load the next level
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1); // Advances to the next level
                Invoke(nameof(LevelLoad), 1.0f); // Loads the next level after a delay of 1 second
            }
        }
    }

    // Loads the current level scene
    public void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die"); // Loads the scene named "Adapt or Die"
    }

    // Processes player shield by disabling it after a fixed time
    public void ProcessPlayerShield()
    {
        Invoke(nameof(DisablePlayerShield), 20.0f); // Disables player shield after 20 seconds
    }

    // Disables the player's shield and update the User Interface
    void DisablePlayerShield()
    {
        PlayerHealth.Instance.playerShield = false; // Sets player's shield status to false
        GameManager.Instance.playerShieldText.SetActive(false); // Hides the shield User Interface text
    }
}
