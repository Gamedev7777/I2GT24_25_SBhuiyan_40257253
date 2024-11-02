using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour

{
    public TextMeshProUGUI scoreText;
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreText.text = "Score: " + PlayerPrefs.GetInt("Score", 0).ToString();
    }
    
    
}
