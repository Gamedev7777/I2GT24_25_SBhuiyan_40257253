using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // public variables
    public bool playerDeath = false; // Variable to check if the player is dead
    public static PlayerHealth
        Instance; // Creates a static instance of the PlayerHealth class to access its properties from other classes
    public bool playerShield = false; // Variable to check if the player shield is active
    [HideInInspector]
    public int health; // Sets the initial health of the player

    void Awake()
    {
        // Assigns the current instance of this script to the static Instance variable
        Instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("Mode", 0) == 0)
        {
            Debug.Log("Easy");
            health = 60;
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1)
        {
            Debug.Log("Normal");
            health = 50;
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2)
        {
            Debug.Log("Hard");
            health = 40;
        }
    }

    // Method to handle when the player takes damage
    public void TakeDamage(int damage)
    {
        // Only takes damage if the player shield is not active
        if (playerShield == false)
        {
            // Reduces health by the value of damage
            health -= damage;

            // Updates the player's health display text in the GameManager User Interface
            GameManager.Instance.playerHealthText.text = "Player Health: " + health.ToString();

            // If health reaches 0 or less and the player is not already dead, calls the Die method
            if (health <= 0 && !playerDeath)
            {
                Die();
            }
        }
    }

    // Method to handle the player's death
    void Die()
    {
        // Logs that the player is dead
        Debug.Log("Player is dead");

        // Loads the next level or handles level restart in the SpawnManager
        SpawnManager.Instance.LevelLoad();

        // Sets playerDeath to true to prevent multiple calls to the Die method
        playerDeath = true;

        // Destroys the player game object
        Destroy(gameObject);
    }
}