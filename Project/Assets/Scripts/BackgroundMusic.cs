using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    public static BackgroundMusic Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BackgroundMusic>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != instance)
            {
                Destroy(gameObject);
            }
        }
    }
}
