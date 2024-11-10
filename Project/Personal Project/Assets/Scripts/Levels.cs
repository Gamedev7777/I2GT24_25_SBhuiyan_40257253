using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    public List<GameObject> aliens = new List<GameObject>();
    public List<GameObject> player = new List<GameObject>();
    public static Levels Instance;

    void Awake()
    {
        Instance = this;
    }
}