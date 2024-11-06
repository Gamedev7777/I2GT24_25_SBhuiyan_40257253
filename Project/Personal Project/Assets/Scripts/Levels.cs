using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public List<GameObject> aliens = new List<GameObject>();
    public GameObject player;
    public static Levels Instance;

    void Awake()
    {
        Instance = this;
     //   Invoke(nameof(PlayGame), 1.0f);
    }

    void PlayGame()
    {
        Time.timeScale = 1;
    }
}
