using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // public variables
    public bool playerDeath = false; // Variable to check if the player is dead

    public static PlayerHealth Instance; // Creates a static instance of the PlayerHealth class to access its properties from other classes

    public bool playerShield = false; // Variable to check if the player shield is active
    [HideInInspector] public int health; // Sets the initial health of the player
    public ParticleSystem playerDeathFX;
    public AudioClip playerDeathSound;
    void Awake()
    {
        // Assigns the current instance of this script to the static Instance variable
        Instance = this;
    }

    // Method to set the player's health based on the selected difficulty mode
    public void SetPlayerHealth()
    {
        // Sets the player's health based on the difficulty mode stored in PlayerPrefs
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            Debug.Log("Easy " + PlayerPrefs.GetInt("Mode", 0));
            health = 60; // Sets health to 60 in easy mode
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            Debug.Log("Normal " + PlayerPrefs.GetInt("Mode", 0));
            health = 50; // Sets health to 50 in normal mode
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
            Debug.Log("Hard " + PlayerPrefs.GetInt("Mode", 0));
            health = 40; // Sets health to 40 in hard mode
        }
    }

    // Method to handle when the player takes damage
    public void TakeDamage(int damage)
    {
        // Only takes damage if the player's shield is not active
        if (playerShield == false)
        {
            // Reduces health by the value of damage
            health -= damage;
           

            // Updates the health display in the User Interface based on the player's name
            if (gameObject.name == "Player" || gameObject.name == "PlayerMultiplayer1")
            {
                GameManager.Instance.playerHealthText.text = "Player 1 Health: " + health.ToString();
            }
            else if (gameObject.name == "PlayerMultiplayer2")
            {
                GameManager.Instance.playerHealthText2.text = "Player 2 Health: " + health.ToString();
            }

            // If health reaches 0 or less and the player is not already dead, calls the Die method
            if (health <= 0 && !playerDeath)
            {
                AudioSource.PlayClipAtPoint(playerDeathSound, Camera.main.transform.position);
                playerDeathFX.Play();
                Invoke(nameof(Die),0.3f);
            }
        }
    }

    // Method to handle the player's death
    void Die()
    {
        // Removes the player from the list of active players in the level
        SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player.Remove(gameObject);
        
        // Loads the next level or handles level restart in the SpawnManager
        SpawnManager.Instance.GameOver();

        // Sets playerDeath to true to prevent multiple calls to the Die method
        playerDeath = true;

        // Destroys the player game object
        Destroy(gameObject);
    }
}
