using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour

{
    public static SpawnManager Instance;
    public GameObject alienPrefab;
    public List<GameObject> alienList = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0;
             i < PlayerPrefs.GetInt("Level", 1);
             i++) // Using for loop and player prefs to spawn multiple aliens. We use player prefs to save data permanently on disk
        {
            alienList.Add(Instantiate(alienPrefab, transform.position,
                Quaternion.identity)); // Instantiating the alien prefab  
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LevelComplete()
    {
        if (alienList.Count <= 0)
        {
            Debug.Log("Level Complete");
            PlayerPrefs.SetInt("Level",
                PlayerPrefs.GetInt("Level", 1) + 1); // Increasing the level number once each level is completed 
            Invoke(nameof(LevelLoad), 3.0f);
        }
    }

    private void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die");
    }
}