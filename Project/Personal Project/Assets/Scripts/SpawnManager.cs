using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    // public variables
    public static SpawnManager Instance; // Singleton instance of SpawnManager for easy access
    public GameObject alienPrefab; // Prefab of the alien to be spawned
    public List<GameObject> alienList = new List<GameObject>(); // List to keep track of all spawned aliens
    public List<Transform> waypoints; // Waypoints that the aliens will follow
    public List<Transform> spawnPoints; // Possible spawn points for aliens

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Sets up the singleton instance
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Spawns a number of aliens based on the current level stored in PlayerPrefs
        for (int i = 0; i < PlayerPrefs.GetInt("Level", 1); i++)
        {
            float offset = i * 2; // Offsets to space out the aliens
            int randomIndex = Random.Range(0, spawnPoints.Count); // Randomly selects a spawn point
            Vector3 spawnPos = spawnPoints[randomIndex].position; // Gets the position of the chosen spawn point
            
            // Instantiates an alien at the chosen spawn position
            GameObject alienSpawned = Instantiate(alienPrefab, spawnPos, Quaternion.identity);
            
            // Adds the newly spawned alien to the alien list
            alienList.Add(alienSpawned);
            
            // Sets the waypoints for the alien's AI
            AlienAI alienAI = alienSpawned.GetComponent<AlienAI>();
            alienAI.waypoints = waypoints;
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
                GameManager.Instance.popUpList[7].SetActive(true);
                PlayerPrefs.SetInt("Level", 1); // Resets level to 1
            }
            else
            {
                // Increments the level and load the next level
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
                Invoke(nameof(LevelLoad), 1.0f); // Loads the next level after a delay of 1 second
            }
        }
    }

    // Loads the current level scene
    public void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die");
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