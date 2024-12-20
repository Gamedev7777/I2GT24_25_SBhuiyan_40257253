using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    // public variables
    public static SpawnManager instance; // Instance of SpawnManager for easy access
    public List<GameObject> alienList = new(); // List to keep track of all spawned aliens
    public GameObject levelSpawned; // Reference to the currently spawned level
    public List<GameObject> levelPrefabList = new(); // List of level prefabs for single-player mode
    public List<GameObject> levelMultiplayerPrefabList = new(); // List of level prefabs for multiplayer mode
    public AudioClip shieldDisabledSound; // Sound played when the shield is disabled
    public Camera fakeCamera; // Reference to the fake camera used during the transition

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Initialises the instance
        instance = this;
    }

    // Method to spawn the level
    public void SpawnLevel()
    {
        SetupCutscene();
        
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            InstantiateSingleplayerLevel();
        }
        // Checks if the game is in multiplayer mode
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            InstantiateMultiplayerLevel();
        }
        
        AddAliensToList();
    }

    private void InstantiateMultiplayerLevel()
    {
        // Instantiates the appropriate level prefab for multiplayer mode
        levelSpawned = Instantiate(levelMultiplayerPrefabList[PlayerPrefs.GetInt("Level", 1) - 1],
            new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void AddAliensToList()
    {
        // All the aliens that spawn are being added to the list of aliens in the SpawnManager script
        for (int i = 0; i < levelSpawned.GetComponent<Levels>().aliens.Count; i++)
        {
            // Adds the newly spawned alien to the alien list
            alienList.Add(levelSpawned.GetComponent<Levels>().aliens[i]);
        }
    }

    private void InstantiateSingleplayerLevel()
    {
        // Instantiates the appropriate level prefab based on the current level stored in PlayerPrefs
        levelSpawned = Instantiate(levelPrefabList[PlayerPrefs.GetInt("Level", 1) - 1], new Vector3(0, 0, 0),
            Quaternion.identity);
    }

    private void SetupCutscene()
    {
        // Setting the value for the cutscene PlayerPrefs and use it to play video before level 1 and cutscenes for other levels
        PlayerPrefs.SetInt("Cutscene", PlayerPrefs.GetInt("Level", 1) == 1 ? 0 : 1);

        fakeCamera.gameObject.SetActive(false); // Deactivate the fake camera
    }

    // Called when the level is completed
    public void LevelComplete()
    {
        // Checks if there is any alien still alive
        if (alienList.Count > 0) return;
        // If the player has completed all 7 levels, shows the final pop-up and reset the level
        if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            UnlockCursor();
            levelSpawned.SetActive(false);
            PlayPostLevel7Video();
            ResetPlayerPrefs();
        }
        else
        {
            // Increments the level and loads the next level
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1); // Advances to the next level
            Invoke(nameof(LevelLoad), 1.0f); // Loads the next level after a delay of 1 second
        }
    }

    private static void ResetPlayerPrefs()
    {
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
        PlayerPrefs.SetInt("Upgraded", 0); // Resets upgrade state
    }

    private static void PlayPostLevel7Video()
    {
        GameManager.instance.videoCamera.SetActive(true); // Turns on the video camera to show video
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

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;
    }

    // Called when the game is over
    public void GameOver()
    {
        // Checks if all players have been eliminated
        if (levelSpawned.GetComponent<Levels>().player.Count <= 0)
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
    private void ProcessAlienTarget()
    {
        var aliens = FindObjectsOfType<AlienAI>(); // Finds all AlienAI instances

        foreach (var alien in aliens)
        {
            alien.target = GameObject.FindGameObjectWithTag("Player"); // Assigns the player as the target for the alien
        }
    }

    // Loads the current level scene
    private void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die"); // Loads the scene named "Adapt or Die"
    }

    // Processes player shield by disabling it after a fixed time
    public void ProcessPlayerShield()
    {
        Invoke(nameof(DisablePlayerShield), 20.0f); // Disables player shield after 20 seconds
    }

    // Disables the player's shield and updates the User Interface
    private void DisablePlayerShield()
    {
        PlayShieldDisabledSound();
        // Checks if the game is in single-player mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            DisableShieldSingleplayerMode();
        }
        // Checks if the game is in multiplayer mode
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                DisableShieldsForBothPlayersMultiplayerMode();
            }
            else
            {
                DisableShieldForAlivePlayerMultiplayerMode();
            }
        }

        // Hides the shield User Interface text
        GameManager.instance.playerShieldText.SetActive(false);
    }

    private void DisableShieldForAlivePlayerMultiplayerMode()
    {
        if (levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
        {
            // Disables the shield for both players
            levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;


            levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield
                .SetActive(false);
        }
        else if (levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
        {
            levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
            levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer2>().claireShield
                .SetActive(false);
        }
    }

    private void DisableShieldsForBothPlayersMultiplayerMode()
    {
        // Disables the shield for both players
        levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;
        levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>().playerShield = false;

        levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield
            .SetActive(false);

        levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerMultiplayer2>().claireShield
            .SetActive(false);
    }

    private void DisableShieldSingleplayerMode()
    {
        // Disables the shield for player 1
        levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = false;

        levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().remyShield.SetActive(false);

        levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().claireShield
            .SetActive(false);
    }

    private void PlayShieldDisabledSound()
    {
        AudioSource.PlayClipAtPoint(shieldDisabledSound, transform.position); // Plays sound when the shield is disabled
    }
}