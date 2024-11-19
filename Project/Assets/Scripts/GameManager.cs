using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // public variables
    // User Interface elements for displaying score, player health, and level number
    public TextMeshProUGUI scoreText;
    public static GameManager Instance; // Singleton instance of GameManager
    public TextMeshProUGUI playerHealthText, playerHealthText2;
    public TextMeshProUGUI levelNumberText, avatarText;
    public GameObject playerShieldText, playerSpeedText; // User Interface elements for player status (shield and speed)
    public List<GameObject> popUpList = new List<GameObject>(); // List of pop-up game objects for different levels
    public GameObject mainMenu; // Reference to the main menu game object
    public GameObject singleAndMultiplayerMenu; // Reference to the single and multiplayer selection menu
    public List<AudioClip> musicList = new List<AudioClip>();
    private int musicIndex;
    public AudioSource musicAudioSource;
    public Slider volumeSlider;
    public GameObject chooseAvatarButton;
    public TextMeshProUGUI chosenAvatarText;
    public GameObject highscoreMenu;
    public TMP_InputField playerNameInputField;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI currentHighscoreText;
    int _mainMode; // Stores the main game mode
    
    
    void Awake()
    {
        // Sets the singleton instance and pauses the game at the start
        Instance = this;
        Time.timeScale = 0; // Pauses the game until the player starts
    }



    public void ChangeMusicVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }



    public void ChooseBackgroundMusic()
    {
        if (musicIndex == 0)
        {
            musicIndex++;
            musicAudioSource.clip = musicList[musicIndex];
            musicAudioSource.Play();
        }
        else if (musicIndex == 1)
        {
            musicIndex++;
            musicAudioSource.clip = musicList[musicIndex];
            musicAudioSource.Play();
        }
        else if (musicIndex == 2)
        {
            musicIndex = 0;
            musicAudioSource.clip = musicList[musicIndex];
            musicAudioSource.Play();
        }
    }
    
    
    
    void Start()
    {
        AudioListener.volume = volumeSlider.value;
        currentHighscoreText.text = "Highscore: " + PlayerPrefs.GetString("PlayerName") + " " + PlayerPrefs.GetInt("Highscore") +" "+ PlayerPrefs.GetString("HighscoreMode");
        ChooseBackgroundMusic();
        
        // Initialises User Interface elements with saved values from PlayerPrefs
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0).ToString(); // Displays the player's score

        levelNumberText.text =
            "Level Number: " + PlayerPrefs.GetInt("Level", 1).ToString(); // Displays the current level number

        // Checks if it is the player's first time playing
        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            singleAndMultiplayerMenu.SetActive(true); // Shows the single and multiplayer selection menu
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
            Debug.Log("remain human");
            popUpList[5].SetActive(false); // Hides the sixth story pop-up
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            popUpList[6].SetActive(false); // Hides the seventh story pop-up
        }

        // Spawns the level using the SpawnManager
        SpawnManager.Instance.SpawnLevel();

        // Handles player health and User Interface elements based on the controller settings
        if (PlayerPrefs.GetInt("Controller", 0) == 0) // Single player mode
        {
            playerHealthText2.gameObject.SetActive(false); // Hides the second player's health User Interface

            PlayerPrefs.SetInt("Mode", _mainMode); // Sets the game mode

            // Sets the player health and updates the User Interface text for player 1
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health 
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1) // Multiplayer mode
        {
            PlayerPrefs.SetInt("Mode", _mainMode); // Sets the game mode

            // Sets the player health and updates the User Interface text for player 1
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health

            // Sets the player health and updates the UI text for player 2
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText2.text = "Player 2 Health: " + SpawnManager.Instance._levelSpawned.GetComponent<Levels>()
                .player[1].GetComponent<PlayerHealth>().health.ToString(); // Displays the player's health
        }

        // Sets the health for each alien in the level
        for (int i = 0; i < SpawnManager.Instance._levelSpawned.GetComponent<Levels>().aliens.Count; i++)
        {
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().aliens[i].GetComponent<AlienHealth>()
                .SetAlienHealth();
        }
        AudioListener.volume = volumeSlider.value;
        // Resumes the game
        Time.timeScale = 1;
    }

    // Method to restart the game and reset the score
    public void PlayAgain()
    {
        
        PlayerPrefs.SetInt("Score", 0); // Resets score to 0
        SceneManager.LoadScene("Adapt or Die"); // Reloads the scene to start again
        
        
    }

    public void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", PlayerPrefs.GetInt("Score", 0));
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            PlayerPrefs.SetString("HighscoreMode", "Easy");
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            PlayerPrefs.SetString("HighscoreMode", "Normal");
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
            PlayerPrefs.SetString("HighscoreMode", "Hard");
        }
        
        
        
        highscoreMenu.SetActive(false);

        if (PlayerPrefs.GetInt("Upgraded", 0) == 1)
        {
            popUpList[8].SetActive(true); // Shows the upgrade story pop-up
        }
        else
        {
            popUpList[7].SetActive(true); // Shows the final level completion pop-up
        }
        
          
        
        
        
        
        
        
    }
    // Method to upgrade the player and show the upgrade story pop-up
    public void Upgrade()
    {
        AudioListener.volume = 0;
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        PlayerPrefs.SetInt("Upgraded", 1);
        if (PlayerPrefs.GetInt("Highscore", 0) < PlayerPrefs.GetInt("Score", 0))
        {
            highscoreMenu.SetActive(true);
            highscoreText.text = "New High Score: " + PlayerPrefs.GetInt("Score", 0).ToString();
        }
        else
        {
            popUpList[8].SetActive(true); // Shows the upgrade story pop-up
        }
    }

    public void ChooseAvatar()
    {
        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            PlayerPrefs.SetInt("Avatar", 1);
            avatarText.text = "Choose Remy";
            chosenAvatarText.text = "Claire chosen";
        }
        else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
        {
            PlayerPrefs.SetInt("Avatar", 0);
            avatarText.text = "Choose Claire";
            chosenAvatarText.text = "Remy chosen";
        }
    }
    
    
    // Method to select the controller type (single or multiplayer)
    public void SelectController(int controllerNumber)
    {
        PlayerPrefs.SetInt("Controller", controllerNumber); // Sets the controller type
        PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
        PlayerPrefs.SetInt("Upgraded", 0);
        singleAndMultiplayerMenu.SetActive(false); // Hides the single and multiplayer menu
        mainMenu.SetActive(true); // Shows the main menu
        if (controllerNumber == 0)
        {
            chooseAvatarButton.SetActive(true);
        }
    }

    // Method called when the Play button is pressed from the main menu
    public void PlayButton(int mode)
    {
        _mainMode = mode; // Sets the game mode

        mainMenu.SetActive(false); // Hides the main menu
    }
}
