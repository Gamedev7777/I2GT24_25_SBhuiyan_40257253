using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour

{
    public TextMeshProUGUI scoreText;
    public static GameManager Instance;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI levelNumberText;
    public GameObject playerShieldText, playerSpeedText;
    public List<GameObject> popUpList = new List<GameObject>();
    public GameObject mainMenu;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 0;
    }

    void Start()
    {
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0).ToString();
        playerHealthText.text = "Player Health: " + PlayerHealth.Instance.health.ToString();
        levelNumberText.text = "Level Number: " + PlayerPrefs.GetInt("Level", 1).ToString();
        if (PlayerPrefs.GetInt("Level", 1) == 1)
        
        {
            mainMenu.SetActive(true);
            popUpList[0].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 2)
        {
            popUpList[1].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 3)
        {
            popUpList[2].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 4)
        {
            popUpList[3].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 5)
        {
            popUpList[4].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 6)
        {
            popUpList[5].SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            popUpList[6].SetActive(true);
        }
    }

    public void StartButton()
    {
        if (PlayerPrefs.GetInt("Level", 1) == 1)
        {
            popUpList[0].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 2)
        {
            popUpList[1].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 3)
        {
            popUpList[2].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 4)
        {
            popUpList[3].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 5)
        {
            popUpList[4].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 6)
        {
            popUpList[5].SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Level", 1) == 7)
        {
            popUpList[6].SetActive(false);
        }
        Time.timeScale = 1;
        
    }

    public void PlayAgain()
    {
        Debug.Log("PlayAgain");
        PlayerPrefs.SetInt("Score", 0);
        SceneManager.LoadScene("Adapt or Die");
        }

    public void Upgrade()
    {
        PlayerPrefs.SetInt("Level", 1);
        Debug.Log("Upgrade");
        popUpList[8].SetActive(true);
    }

    public void PlayButton()
    {
        mainMenu.SetActive(false);
        Time.timeScale = 1;
    }
    
}