using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public List<GameObject> aliens = new List<GameObject>(); // List of alien game objects in the level
    public List<GameObject> player = new List<GameObject>(); // List of player game objects in the level
    public static Levels Instance; // Singleton instance of Levels

    void Awake()
    {
        // Assigns the instance of this script to the static Instance variable
        Instance = this;
    }
}