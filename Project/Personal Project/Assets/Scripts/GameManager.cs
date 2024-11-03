using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour

{
    public TextMeshProUGUI scoreText;
    public static GameManager Instance;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI levelNumberText;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0).ToString();
        playerHealthText.text = "Player Health: " + PlayerHealth.Instance.health.ToString();
        levelNumberText.text = "Level Number: " + PlayerPrefs.GetInt("Level", 1).ToString();

    }
    
    
}
