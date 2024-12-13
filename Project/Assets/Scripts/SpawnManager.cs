using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    // public variables
    public static SpawnManager instance; // Singleton instance of SpawnManager for easy access
    public List<GameObject> alienList = new List<GameObject>(); // List to keep track of all spawned aliens
    public GameObject _levelSpawned; // Reference to the currently spawned level
    public List<GameObject> levelPrefabList = new List<GameObject>(); // List of level prefabs for single-player mode

    public List<GameObject>
        levelMultiplayerPrefabList = new List<GameObject>(); // List of level prefabs for multiplayer mode

    public AudioClip shieldDisabledSound; // Sound played when the shield is disabled
    public Camera fakeCamera; // Reference to the fake camera used during the transition

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Sets up the singleton instance
        instance = this;
    }

    // Method to spawn the level
    public void SpawnLevel()
    {
        if (PlayerPrefs.GetInt("Level", 1) == 1)
        {
            PlayerPrefs.SetInt("Cutscene", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Cutscene", 1);
        }

        fakeCamera.gameObject.SetActive(false); // Deactivate the fake camera
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
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
            AudioListener.volume = 0; // Mutes the audio
            // If the player has completed all 7 levels, shows the final pop-up and reset the level
            if (PlayerPrefs.GetInt("Level", 1) == 7)
            {
                // Checks if the player has achieved a new high score
                if (PlayerPrefs.GetInt("Highscore", 0) < PlayerPrefs.GetInt("Score", 0))
                {
                    Cursor.lockState = CursorLockMode.None; // Lock the cursor to the center of the game window.
                    Cursor.visible = true;
                    _levelSpawned.SetActive(false);
                    GameManager.instance.videoCamera.SetActive(true);
                    GameManager.instance.popUpList[7].SetActive(true); // Shows the upgrade story pop-up
                    
                    if (PlayerPrefs.GetInt("Avatar", 0) == 0)
                    {
                        // Remy is chosen
                        GameManager.instance.videoPlayer8.clip = GameManager.instance.remyVideoClip8;

                    }
                    else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
                    {
                        // Claire is chosen
                        GameManager.instance.videoPlayer8.clip = GameManager.instance.claireVideoClip8;

                    }
                    
                    GameManager.instance.videoPlayer8.gameObject.SetActive(true);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None; // Unlock the cursor to the center of the game window.
                    Cursor.visible = true;
                    GameManager.instance.popUpList[7].SetActive(true); // Shows the final level completion pop-up
                }

                PlayerPrefs.SetInt("Level", 1); // Resets level to 1
                PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
                PlayerPrefs.SetInt("Upgraded", 0); // Resets upgrade state
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
            fakeCamera.gameObject.SetActive(true); // Activates the fake camera
            Invoke(nameof(LevelLoad), 1.0f); // Loads the level scene after a delay of 1 second
        }
    }

    // Assigns target for all aliens in multiplayer mode
    public void AssignAlienTargetForMultiplayer()
    {
        Invoke(nameof(ProcessAlienTarget), 1.0f); // Delays assigning targets by 1 second
    }

    // Processes and assigns a player as the target for each alien in multiplayer mode
    void ProcessAlienTarget()
    {
        var aliens = FindObjectsOfType<AlienAI>(); // Finds all AlienAI instances

        foreach (var alien in aliens)
        {
            alien.target = GameObject.FindGameObjectWithTag("Player"); // Assigns the player as the target for the alien
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
        AudioSource.PlayClipAtPoint(shieldDisabledSound, transform.position); // Plays sound when the shield is disabled
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Disables the shield for player 1
            _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;

            _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().remyShield.SetActive(false);

            _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().claireShield
                .SetActive(false);
        }
        // Checks if the game is in multiplayer mode
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (_levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                // Disables the shield for both players
                _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
                _levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>().playerShield = false;

                _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield
                    .SetActive(false);

                _levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerMultiplayer2>().claireShield
                    .SetActive(false);
            }
            else
            {
                if (_levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    // Disables the shield for both players
                    _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;


                    _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield
                        .SetActive(false);
                }
                else if (_levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
                    _levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer2>().claireShield
                        .SetActive(false);
                }
            }
        }

        // Hides the shield User Interface text
        GameManager.instance.playerShieldText.SetActive(false);
    }
}