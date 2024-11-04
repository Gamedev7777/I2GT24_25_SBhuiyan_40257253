using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour

{
    public static SpawnManager Instance;
    public GameObject alienPrefab;
    public List<GameObject> alienList = new List<GameObject>();
    public List<Transform> waypoints;
    public List<Transform> spawnPoints;
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
            float offset = i * 2;
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Vector3 spawnPos = spawnPoints[randomIndex].position; // Offsets position of alien so it is not in the ground
            GameObject alienSpawned = Instantiate(alienPrefab, spawnPos, Quaternion.identity);
            alienList.Add(alienSpawned); // Instantiating the alien prefab 
            AlienAI alienAI = alienSpawned.GetComponent<AlienAI>();
            alienAI.waypoints = waypoints;
        }
    }

    public void LevelComplete()
    {
        if (alienList.Count <= 0)
        {
            if (PlayerPrefs.GetInt("Level", 1) == 7)
            {
                GameManager.Instance.popUpList[7].SetActive(true);
                PlayerPrefs.SetInt("Level", 1);
            }
            else
            {
            
                PlayerPrefs.SetInt("Level",
                    PlayerPrefs.GetInt("Level", 1) + 1); // Increasing the level number once each level is completed 
                Invoke(nameof(LevelLoad), 1.0f);
            }
        }
    }

    public void LevelLoad()
    {
        SceneManager.LoadScene("Adapt or Die");
    }

    public void ProcessPlayerShield()
    {
        Invoke(nameof(DisablePlayerShield), 20.0f);
    }

    void DisablePlayerShield()
    {
        PlayerHealth.Instance.playerShield = false;
        GameManager.Instance.playerShieldText.SetActive(false);
    }
}