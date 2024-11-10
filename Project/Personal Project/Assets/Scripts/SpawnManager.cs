using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    // public variables
    public static SpawnManager Instance; // Singleton instance of SpawnManager for easy access
    public List<GameObject> alienList = new List<GameObject>(); // List to keep track of all spawned aliens
    public GameObject _levelSpawned; // Reference to the currently spawned level
    public List<GameObject> levelPrefabList = new List<GameObject>(); // List of level prefabs for single-player mode
    public List<GameObject> levelMultiplayerPrefabList = new List<GameObject>(); // List of level prefabs for multiplayer mode

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Sets up the singleton instance
        Instance = this;
    }

    // Method to spawn the level
    public void SpawnLevel()
    {
        Debug.Log("Test");
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            Debug.Log("Singleplayer " + PlayerPrefs.GetInt("Level", 1));
            // Instantiates the appropriate level prefab based on the current level stored in PlayerPrefs
            _levelSpawned = Instantiate(levelPrefabList[PlayerPrefs.GetInt("Level", 1) - 1], new Vector3(0, 0, 0),
                Quaternion.identity);
            // Spawns a number of aliens based on the current level stored in PlayerPrefs
            for (int i = 0; i < _levelSpawned.GetComponent<Levels>().aliens.Count; i++)
            {
                // Adds the newly spawned alien to the alien list
                alienList.Add(_levelSpawned.GetComponent<Levels>().aliens[i]);
            }
        }
        // Checks if the game is in multiplayer mode
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            Debug.Log("Multiplayer " + PlayerPrefs.GetInt("Level", 1));
            // Instantiates the appropriate level prefab for multiplayer mode
            _levelSpawned = Instantiate(levelMultiplayerPrefabList[PlayerPrefs.GetInt("Level", 1) - 1],
                new Vector3(0, 0, 0), Quaternion.identity);
            // Spawns a number of aliens based on the current level stored in PlayerPrefs
            for (int i = 0; i < _levelSpawned.GetComponent<Levels>().aliens.Count; i++)
            {
                // Adds the newly spawned alien to the alien list
                alienList.Add(_levelSpawned.GetComponent<Levels>().aliens[i]);
            }
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
                PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
            }
            else
            {
                // Increments the level and loads the next level
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1); // Advances to the next level
                Invoke(nameof(LevelLoad), 1.0f); // Loads the next level after a delay of 1 second
            }
        }
    }

    // Called when the game is over
    public void GameOver()
    {
        // Checks if all players have been eliminated
        if (_levelSpawned.GetComponent<Levels>().player.Count <= 0)
        {
            Invoke(nameof(LevelLoad), 1.0f); // Loads the next level after a delay of 1 second
        }
    }

    // Loads the current level scene
    void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die"); // Loads the scene named "Adapt or Die"
    }

    // Processes player shield by disabling it after a fixed time
    public void ProcessPlayerShield()
    {
        Invoke(nameof(DisablePlayerShield), 20.0f); // Disables player shield after 20 seconds
    }

    // Disables the player's shield and updates the User Interface
    void DisablePlayerShield()
    {
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Disables the shield for player 1
            _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
        }
        // Checks if the game is in multiplayer mode
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            // Disables the shield for both players
            _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
            _levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>().playerShield = false;
        }

        // Hides the shield User Interface text
        GameManager.Instance.playerShieldText.SetActive(false);
    }
}
