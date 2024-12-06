using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    // public variables
    // User Interface elements for displaying score, player health, and level number
    public TextMeshProUGUI scoreText;
    public static GameManager instance; // Singleton instance of GameManager
    public TextMeshProUGUI playerHealthText, playerHealthText2; // User Interface for player 1 and player 2 health
    public TextMeshProUGUI levelNumberText, avatarText; // User Interface for level number and avatar selection text
    public GameObject playerShieldText, playerSpeedText, videoCamera; // User Interface elements for player status (shield and speed)
    public List<GameObject> popUpList = new List<GameObject>(); // List of pop-up game objects for different levels
    public GameObject mainMenu; // Reference to the main menu game object
    public GameObject singleAndMultiplayerMenu; // Reference to the single and multiplayer selection menu
    public List<AudioClip> musicList = new List<AudioClip>(); // List of background music clips
    public AudioSource musicAudioSource; // AudioSource component to play background music
    public Slider volumeSlider; // Slider User Interface element for controlling game music volume
    public GameObject chooseAvatarButton; // Button to allow player to choose an avatar
    public TextMeshProUGUI chosenAvatarText; // User Interface text showing the chosen avatar name
    public GameObject highscoreMenu; // User Interface for high score input and display
    public TMP_InputField playerNameInputField; // Input field for player name in high score menu
    public TextMeshProUGUI highscoreText; // User Interface text for displaying new high score
    public TextMeshProUGUI currentHighscoreText; // User Interface text for displaying current high score
    public VideoPlayer videoPlayer8, videoPlayer9;
    // private variables
    private int _level; // Holds the current level number
    private int _musicIndex; // Holds the current music index for changing background music

    void Awake()
    {
        // Sets the singleton instance and pauses the game at the start
        instance = this; // Assigns the singleton instance to this object
        Time.timeScale = 0; // Pauses the game until the player starts
    }

    // Method to change the volume of the game music
    public void ChangeMusicVolume()
    {
        AudioListener.volume = volumeSlider.value; // Sets the game volume based on the slider value
    }

    // Method to change the background music
    public void ChooseBackgroundMusic()
    {
        // Changes music based on the current index, looping back to the first track after the last one
        if (_musicIndex == 0)
        {
            _musicIndex++;
            musicAudioSource.clip = musicList[_musicIndex];
            musicAudioSource.Play();
        }
        else if (_musicIndex == 1)
        {
            _musicIndex++;
            musicAudioSource.clip = musicList[_musicIndex];
            musicAudioSource.Play();
        }
        else if (_musicIndex == 2)
        {
            _musicIndex = 0;
            musicAudioSource.clip = musicList[_musicIndex];
            musicAudioSource.Play();
        }
    }

    void Start()
    {
        // Retrieves and initialises saved game settings and User Interface
        _level = PlayerPrefs.GetInt("Level", 1);
        AudioListener.volume = volumeSlider.value; // Sets the initial volume from the slider value
        currentHighscoreText.text = "Highscore: " + PlayerPrefs.GetString("PlayerName") + " " +
                                    PlayerPrefs.GetInt("Highscore") + " " + PlayerPrefs.GetString("HighscoreMode");
        ChooseBackgroundMusic(); // Plays background music

        // Initialises User Interface elements with saved values from PlayerPrefs
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0); // Displays the player's score

        levelNumberText.text =
            "Level Number: " + _level; // Displays the current level number

        // Checks if it is the player's first time playing
        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            singleAndMultiplayerMenu.SetActive(true); // Shows the single and multiplayer selection menu
        }

        // Displays the appropriate pop-up based on the current level
        if (_level == 1)
        {
            popUpList[0].SetActive(true); // Shows the first story pop-up
        }
        else if (_level == 2)
        {
            popUpList[1].SetActive(true); // Shows the second story pop-up for level 2
            StartButton();
        }
        else if (_level == 3)
        {
            popUpList[2].SetActive(true); // Shows the third story pop-up for level 3
            StartButton();
        }
        else if (_level == 4)
        {
            popUpList[3].SetActive(true); // Shows the fourth story pop-up for level 4
            StartButton();
        }
        else if (_level == 5)
        {
            popUpList[4].SetActive(true); // Shows the fifth story pop-up for level 5
            StartButton();
        }
        else if (_level == 6)
        {
            popUpList[5].SetActive(true); // Shows the sixth story pop-up for level 6
            StartButton();
        }
        else if (_level == 7)
        {
            popUpList[6].SetActive(true); // Shows the seventh story pop-up for level 7
            StartButton();
        }
    }

    public void SkipCutscene()
    {
        var cutsceneCamera = SpawnManager.instance._levelSpawned.GetComponentInChildren<CutsceneCamera>();
        cutsceneCamera.SwitchCameras();
        
    }
    
    
    
    
    
    
    
    
    
    // Method called when the Start button is pressed
    public void StartButton()
    {
        videoCamera.SetActive(false);
        // Deactivates level pop-ups except for the final ones
        if (PlayerPrefs.GetInt("Level", 1) == 1) 
        {
            for (int i = 0; i < popUpList.Count - 2; i++)
            {
                popUpList[i].SetActive(false);
            }
            Cursor.lockState = CursorLockMode.Locked; // Unlock the cursor, allowing free movement outside the game window.
            Cursor.visible = false;
        }
        

        // Spawns the level using the SpawnManager
        SpawnManager.instance.SpawnLevel();

        // Handles player health and User Interface elements based on the game mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0) // Single player mode
        {
            playerHealthText2.gameObject.SetActive(false); // Hides the second player's health User Interface

            PlayerPrefs.SetInt("Mode", PlayerPrefs.GetInt("Mode", 0)); // Sets the game mode

            // Sets the player health and updates the User Interface text for player 1
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health; // Displays the player's health 
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1) // Multiplayer mode
        {
            PlayerPrefs.SetInt("Mode", PlayerPrefs.GetInt("Mode", 0)); // Sets the game mode

            // Sets the player health and updates the User Interface text for player 1
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText.text =
                "Player 1 Health: " + SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerHealth>().health; // Displays the player's health

            // Sets the player health and updates the UI text for player 2
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                .SetPlayerHealth();
            playerHealthText2.text = "Player 2 Health: " + SpawnManager.instance._levelSpawned.GetComponent<Levels>()
                .player[1].GetComponent<PlayerHealth>().health; // Displays the player's health
        }

        // Sets the health for each alien in the level
        for (int i = 0; i < SpawnManager.instance._levelSpawned.GetComponent<Levels>().aliens.Count; i++)
        {
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().aliens[i].GetComponent<AlienHealth>()
                .SetAlienHealth();
        }

        AudioListener.volume = volumeSlider.value; // Sets volume to the slider value
        Time.timeScale = 1; // Resumes the game
    }

    // Method to restart the game and reset the score
    public void PlayAgain()
    {
        if (PlayerPrefs.GetInt("Highscore", 0) < PlayerPrefs.GetInt("Score", 0))
        {
            highscoreMenu.SetActive(true); // Displays the high score menu if new high score is achieved
            highscoreText.text = "New High Score: " + PlayerPrefs.GetInt("Score", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Level", 1); // Resets level to 1
            PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
            PlayerPrefs.SetInt("Upgraded", 0); // Resets upgrade state
            
            
            PlayerPrefs.SetInt("Score", 0); // Resets score to 0
            SceneManager.LoadScene("Adapt or Die"); // Reloads the scene to start again
        }
        SpawnManager.instance.fakeCamera.gameObject.SetActive(true);
    }

    // Method to save the high score and player information
    public void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", PlayerPrefs.GetInt("Score", 0)); // Saves the current score as high score
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text); // Saves player name

        // Saves the game mode that achieved the high score
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

        highscoreMenu.SetActive(false); // Hides the high score menu

       // if (PlayerPrefs.GetInt("Upgraded", 0) == 1)
        {
            PlayerPrefs.SetInt("firstTime", 1); // Marks as first time playing
            PlayerPrefs.SetInt("Score", 0); // Resets score to 0
            SceneManager.LoadScene("Adapt or Die"); // Reloads the scene to start again
        }
        // else
        // {
        //     popUpList[7].SetActive(true); // Shows the final level completion pop-up
        // }
    }

    // Method to upgrade the player and show the upgrade story pop-up
    public void Upgrade()
    {
        AudioListener.volume = 0; // Mutes volume during upgrade
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        PlayerPrefs.SetInt("Upgraded", 1); // Marks player as upgraded
        
        Destroy(SpawnManager.instance._levelSpawned);
        videoCamera.SetActive(true);
            popUpList[5].SetActive(false);
            popUpList[8].SetActive(true); // Shows the upgrade story pop-up
            videoPlayer9.gameObject.SetActive(true);
        
    }

    // Method to toggle between avatars
    public void ChooseAvatar()
    {
        // Toggles the avatar selection between Claire and Remy
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

    // Method to select the game mode (single or multiplayer)
    public void SelectController(int controllerNumber)
    {
        PlayerPrefs.SetInt("Controller", controllerNumber); // Sets the controller type
        PlayerPrefs.SetInt("firstTime", 0); // Marks that the first-time setup is complete
        PlayerPrefs.SetInt("Upgraded", 0); // Resets upgraded status
        singleAndMultiplayerMenu.SetActive(false); // Hides the single and multiplayer menu
        mainMenu.SetActive(true); // Shows the main menu

        // If single player mode is selected, shows avatar selection
        if (controllerNumber == 0)
        {
            chooseAvatarButton.SetActive(true);
            chosenAvatarText.gameObject.SetActive(true);
        }
    }

    // Method called when the Play button is pressed from the main menu
    public void PlayButton(int mode)
    {
        PlayerPrefs.SetInt("Mode", mode); // Sets the game difficulty level (0: Easy, 1: Normal, 2: Hard)
        mainMenu.SetActive(false); // Hides the main menu
    }
}
