using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // public variables
    // User Interface elements for displaying score, player health, and level number
    public TextMeshProUGUI scoreText;
    public static GameManager Instance; // Singleton instance of GameManager
    public TextMeshProUGUI playerHealthText, playerHealthText2;
    public TextMeshProUGUI levelNumberText;
    public GameObject playerShieldText, playerSpeedText; // User Interface elements for player status (shield and speed)
    public List<GameObject> popUpList = new List<GameObject>(); // List of pop-up game objects for different levels
    public GameObject mainMenu; // Reference to the main menu game object
    public GameObject singleAndMultiplayerMenu;

    void Awake()
    {
        // Sets the singleton instance and pauses the game at the start
        Instance = this;
        Time.timeScale = 0; // Pauses the game until the player starts
    }

    void Start()
    {
        // Initialises User Interface elements with saved values from PlayerPrefs
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0).ToString(); // Displays the player's score

        levelNumberText.text =
            "Level Number: " + PlayerPrefs.GetInt("Level", 1).ToString(); // Displays the current level number


        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            singleAndMultiplayerMenu.SetActive(true);
        }


        // Displays the appropriate pop-up based on the current level
        if (PlayerPrefs.GetInt("Level", 1) == 1)

        {
            popUpList[0].SetActive(true); // Shows the first story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 2)
        {
            popUpList[1].SetActive(true); // Shows the second story pop-up for level 2
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 3)
        {
            popUpList[2].SetActive(true); // Shows the third story pop-up for level 3
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 4)
        {
            popUpList[3].SetActive(true); // Shows the fourth story pop-up for level 4
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 5)
        {
            popUpList[4].SetActive(true); // Shows the fifth story pop-up for level 5
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 6)
        {
            popUpList[5].SetActive(true); // Shows the sixth story pop-up for level 6
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            popUpList[6].SetActive(true); // Shows the seventh story pop-up for level 7
        }
    }

    // Method called when the Start button is pressed
    public void StartButton()
    {
        // Hides the appropriate pop-up based on the current level
        if (PlayerPrefs.GetInt("Level", 1) == 1)
        {
            popUpList[0].SetActive(false); // Hides the first story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 2)
        {
            popUpList[1].SetActive(false); // Hides the second story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 3)
        {
            popUpList[2].SetActive(false); // Hides the third story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 4)
        {
            popUpList[3].SetActive(false); // Hides the fourth story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 5)
        {
            popUpList[4].SetActive(false); // Hides the fifth story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 6)
        {
            popUpList[5].SetActive(false); // Hides the sixth story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            popUpList[6].SetActive(false); // Hides the seventh story pop-up
        }

        SpawnManager.Instance.SpawnLevel();
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            playerHealthText2.gameObject.SetActive(false);

            PlayerPrefs.SetInt("Mode", _mainMode);

            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health 
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)

        {
            PlayerPrefs.SetInt("Mode", _mainMode);
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();


            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health


            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText2.text = "Player 2 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>()
                .player[1].GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health
        }


        for (int i = 0; i < SpawnManager.Instance._levelSpawned.GetComponent<Levels>().aliens.Count; i++)
        {
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().aliens[i].GetComponent<AlienHealth>()
                .SetAlienHealth();
        }


        // Resumes the game
        Time.timeScale = 1;
    }

    // Method to restart the game and reset the score
    public void PlayAgain()
    {
        Debug.Log("PlayAgain"); // Logs the PlayAgain action
        PlayerPrefs.SetInt("Score", 0); // Resets score to 0
        SceneManager.LoadScene("Adapt or Die"); // Reloads the scene to start again
    }

    // Method to upgrade the player and show the upgrade story pop-up
    public void Upgrade()
    {
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        Debug.Log("Upgrade"); // Logs the Upgrade action
        popUpList[8].SetActive(true); // Shows the upgrade story pop-up
    }

    public void SelectController(int controllerNumber)
    {
        PlayerPrefs.SetInt("Controller", controllerNumber);
        PlayerPrefs.SetInt("firstTime", 0);
        singleAndMultiplayerMenu.SetActive(false);
        mainMenu.SetActive(true);
        //SpawnManager.Instance.SpawnLevel();
    }

    int _mainMode;

    // Method called when the Play button is pressed from the main menu
    public void PlayButton(int mode)
    {
        _mainMode = mode;


        mainMenu.SetActive(false); // Hides the main menu
    }
}