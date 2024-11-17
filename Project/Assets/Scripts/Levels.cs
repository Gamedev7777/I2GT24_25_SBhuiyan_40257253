using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public List<GameObject> aliens = new List<GameObject>(); // List of alien game objects in the level
    public List<GameObject> player = new List<GameObject>(); // List of player game objects in the level
    public static Levels Instance; // Singleton instance of Levels
    public GameObject remy, claire;
    
    void Awake()
    {
        // Assigns the instance of this script to the static Instance variable
        Instance = this;
    }


    void Start()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            if (PlayerPrefs.GetInt("Avatar", 0) == 0)
            {
                remy.SetActive(true);
                claire.SetActive(false);
            }
            else if (PlayerPrefs.GetInt("Avatar", 0) == 1)
            {
                remy.SetActive(false);
                claire.SetActive(true);
            }
        }
        
    }
}