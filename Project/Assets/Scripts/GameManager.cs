using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    // public variables
    public TextMeshProUGUI scoreText; // User Interface element for displaying score
    public static GameManager instance; // Instance of GameManager
    public TextMeshProUGUI playerHealthText, playerHealthText2; // User Interface for player 1 and player 2 health
    public TextMeshProUGUI levelNumberText, avatarText; // User Interface for level number and avatar selection text
    public GameObject playerShieldText, playerSpeedText; // User Interface elements for player status (shield and speed)
    public GameObject videoCamera; // Video camera is used to show the videos
    public List<GameObject> popUpList = new(); // List of pop-up game objects for different levels
    public GameObject mainMenu; // Reference to the main menu game object
    public GameObject singleAndMultiplayerMenu; // Reference to the single and multiplayer selection menu
    public List<AudioClip> musicList = new(); // List of background music clips
    public List<Slider> volumeSlider = new(); // Slider User Interface element for controlling game music volume
    public GameObject chooseAvatarButton; // Button to allow player to choose an avatar
    public TextMeshProUGUI chosenAvatarText; // User Interface text showing the chosen avatar name
    public GameObject highscoreMenu; // User Interface for high score input and display
    public TMP_InputField playerNameInputField; // Input field for player name in high score menu
    public TextMeshProUGUI highscoreText; // User Interface text for displaying new high score
    public TextMeshProUGUI currentHighscoreText; // User Interface text for displaying current high score
    public VideoPlayer videoPlayer1, videoPlayer8, videoPlayer9; // Video players to play story part 8 and 9 videos
    public VideoClip remyVideoClip1, claireVideoClip1, remyVideoClip8, claireVideoClip8, remyVideoClip9, claireVideoClip9; // Variables for video clips
    public AudioClip buttonAudioClip; // Variable for button click sound
    [HideInInspector] public AudioSource musicAudioSource; // AudioSource component to play background music
    
    // private variables
    private int _level; // Holds the current level number
    private int _musicIndex; // Holds the current music index for changing background music
    private bool _isPaused; // Checks if the game is paused or not
    [SerializeField] private GameObject pauseMenu; // Pause Menu User Interface
    private readonly float _sceneLoadDelay = 0.6f; // Delay for loading the scene

    private void Awake()
    {
        instance = this;
        Time.timeScale = 0; // Stops the game
    }

    public void ChangeMusicVolume(int index)
    {
        for (int i = 0; i < volumeSlider.Count; i++)
        {
            volumeSlider[i].value = volumeSlider[index].value;
        }
        AudioListener.volume = volumeSlider[index].value; // Sets the game volume based on the slider value
    }

    public void ChooseBackgroundMusic()
    {
        // Changes music based on the current index, looping back to the first track after the last one
        if (_musicIndex == 0)
        {
            _musicIndex++;
            ChangeBackgroundMusicClip();
        }
        else if (_musicIndex == 1)
        {
            _musicIndex++;
            ChangeBackgroundMusicClip();
        }
        else if (_musicIndex == 2)
        {
            _musicIndex = 0;
            ChangeBackgroundMusicClip();
        }

        PlayButtonClickSound();
    }

    private void PlayButtonClickSound()
    {
        AudioSource.PlayClipAtPoint(buttonAudioClip, SpawnManager.instance.transform.position);
    }

    private void ChangeBackgroundMusicClip()
    {
        PlayerPrefs.SetInt("musicindex", _musicIndex);
        musicAudioSource.clip = musicList[_musicIndex];
        musicAudioSource.Play();
    }

    private void Start()
    {
        _musicIndex = PlayerPrefs.GetInt("musicindex", 0);

        InitialiseTheMusic();

        GetCurrentLevel();

        InitialiseUITexts();

        ChangeBackgroundMusicClip();

        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            singleAndMultiplayerMenu.SetActive(true);
        }

        InitialiseLevel();
    }

    private void InitialiseLevel()
    {
        popUpList[_level - 1].SetActive(true); // Turns on the relevant pop-up based on the level number
        
        if (_level > 1)
        {
            StartButton(false);
        }
    }

    private void InitialiseUITexts()
    {
        currentHighscoreText.text = "Highscore: " + PlayerPrefs.GetString("PlayerName") + " " +
                                    PlayerPrefs.GetInt("Highscore") + " " + PlayerPrefs.GetString("HighscoreMode");
        
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0);

        levelNumberText.text = "Level Number: " + _level; 
    }

    private void GetCurrentLevel()
    {
        _level = PlayerPrefs.GetInt("Level", 1);
    }

    private void InitialiseTheMusic()
    {
        musicAudioSource = FindObjectOfType<BackgroundMusic>().GetComponent<AudioSource>();
        ChangeMusicVolume(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isPaused && SpawnManager.instance.levelSpawned !=null) // Checking for Esc key to be pressed and when the game is not paused
        {
            _isPaused = true;
            GamePaused();
        }
    }

    private void GamePaused()
    {
        UnlockCursor();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    private static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        LockCursor();
        PlayButtonClickSound();
    }

    private static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("Level", 1); 
        PlayerPrefs.SetInt("firstTime", 1); 
        PlayerPrefs.SetInt("Upgraded", 0); 
        PlayButtonClickSound();
        Invoke(nameof(ProcessLoadScene), _sceneLoadDelay);
    }

    private void ProcessLoadScene()
    {
        SceneManager.LoadScene("Adapt or Die");
    }

    public void SkipCutscene()
    {
        var cutsceneCamera = SpawnManager.instance.levelSpawned.GetComponentInChildren<CutsceneCamera>();
        cutsceneCamera.SwitchCameras();
        PlayButtonClickSound();
    }

    public void StartButton(bool clicked)
    {
        videoCamera.SetActive(false);
        
        InitialisePopups();
    
        // Spawns the level using the SpawnManager
        SpawnManager.instance.SpawnLevel();

        InitialiseModeAndPlayerHealth();

        SetAliensHealth();

        ChangeMusicVolume(0);
        
        Time.timeScale = 1; // Resumes the game
        
        if (clicked)
        {
            PlayButtonClickSound();
        }
    }

    private void InitialiseModeAndPlayerHealth()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0) // Single player mode
        {
            SetGameMode();

            SetSinglePlayerHealth();
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1) // Multiplayer mode
        {
            SetGameMode();

            SetMultiplayerHealth();
        }
    }

    private void InitialisePopups()
    {
        if (PlayerPrefs.GetInt("Level", 1) == 1) 
        {
            // Deactivates level pop-ups except for the final one
            for (int i = 0; i < popUpList.Count - 2; i++)
            {
                popUpList[i].SetActive(false);
            }

            LockCursor();
        }
    }

    private static void SetAliensHealth()
    {
        foreach (GameObject alien in SpawnManager.instance.levelSpawned.GetComponent<Levels>().aliens)
        {
            alien.GetComponent<AlienHealth>().SetAlienHealth();
        }
    }

    private void SetMultiplayerHealth()
    {
        PlayerHealth player0 = SpawnManager.instance.levelSpawned.GetComponent<Levels>().player[0]
            .GetComponent<PlayerHealth>();
        PlayerHealth player1 = SpawnManager.instance.levelSpawned.GetComponent<Levels>().player[1]
            .GetComponent<PlayerHealth>();

        // Sets the player health
        player0.SetPlayerHealth();
        playerHealthText.text = "Player 1 Health: " + player0.health; // Displays the player's health

        // Sets the player health
        player1.SetPlayerHealth();
        playerHealthText2.text = "Player 2 Health: " + player1.health; // Displays the player's health
    }
    
    private void SetSinglePlayerHealth()
    {
        PlayerHealth player0 = SpawnManager.instance.levelSpawned.GetComponent<Levels>().player[0]
            .GetComponent<PlayerHealth>(); // Gets player health component

        playerHealthText2.gameObject.SetActive(false); // Hides the second player's health User Interface

        // Sets the player health and updates the User Interface text for player 1
        player0.SetPlayerHealth();
        playerHealthText.text = "Player 1 Health: " + player0.health; // Displays the player's health 
    }
    
    private static void SetGameMode()
    {
        PlayerPrefs.SetInt("Mode", PlayerPrefs.GetInt("Mode", 0)); // Sets the game mode
    }

    // Method to restart the game and reset the score
    public void PlayAgain()
    {
        if (IsHighScoreAchieved())
        {
            highscoreMenu.SetActive(true); // Displays the high score menu if new high score is achieved
            highscoreText.text = "New High Score: " + PlayerPrefs.GetInt("Score", 0);
        }
        else
        {
            ResetThePlayerPrefs();

            Invoke(nameof(ProcessLoadScene), _sceneLoadDelay);
        }

        SpawnManager.instance.fakeCamera.gameObject.SetActive(true); // Fake camera used to prevent no camera rendering error
        PlayButtonClickSound();
    }

    private static void ResetThePlayerPrefs()
    {
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        PlayerPrefs.SetInt("Upgraded", 0); // Resets upgrade state
        PlayerPrefs.SetInt("Score", 0); // Resets score to 0
        PlayerPrefs.SetInt("firstTime", 1); // Marks as first time playing
    }

    private static bool IsHighScoreAchieved()
    {
        return PlayerPrefs.GetInt("Highscore", 0) < PlayerPrefs.GetInt("Score", 0);
    }

    // Method to save the high score and player information
    public void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", PlayerPrefs.GetInt("Score", 0)); // Saves the current score as high score
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text); // Saves player name

        SaveHighscoreMode();

        highscoreMenu.SetActive(false); // Hides the high score menu
        ResetThePlayerPrefs();
        PlayButtonClickSound();

        Invoke(nameof(ProcessLoadScene), _sceneLoadDelay);
    }

    private static void SaveHighscoreMode()
    {
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
    }

    public void DeleteHighscore()
    {
        PlayerPrefs.SetInt("Score", 0);
        currentHighscoreText.text = "Highscore: " + 0;
    }
    
    // Method to upgrade the player and show the upgrade story pop-up
    public void Upgrade()
    {
        PlayerPrefs.SetInt("Level", 1); // Resets level to 1
        PlayerPrefs.SetInt("Upgraded", 1); // Marks player as upgraded

        Destroy(SpawnManager.instance.levelSpawned); // Destroys level as the Be Upgraded video is shown
        ShowUpgradeVideo();
        PlayButtonClickSound();
    }

    private void ShowUpgradeVideo()
    {
        videoCamera.SetActive(true); // Camera is being turned on so players can view the Upgrade video
        popUpList[5].SetActive(false);
        popUpList[8].SetActive(true); 
        
        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            // Remy is chosen
            videoPlayer9.clip = remyVideoClip9;
        }
        else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
        {
            // Claire is chosen
            videoPlayer9.clip = claireVideoClip9;
        }
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
        PlayButtonClickSound();
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

        PlayButtonClickSound();
    }

    // Method called when the Play button is pressed from the main menu
    public void PlayButton(int mode)
    {
        PlayerPrefs.SetInt("Mode", mode); // Sets the game difficulty level (0: Easy, 1: Normal, 2: Hard)
        mainMenu.SetActive(false); // Hides the main menu
        PlayButtonClickSound();
    }

    public void PlayVideo1()
    {
        if (PlayerPrefs.GetInt("Avatar", 0) == 0)
        {
            // Remy is chosen
            videoPlayer1.clip = remyVideoClip1;
            videoPlayer1.Play();
        }
        else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
        {
            // Claire is chosen
            videoPlayer1.clip = claireVideoClip1;
            videoPlayer1.Play();
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game
    }
}