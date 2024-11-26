using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Public variables
    public bool playerDeath = false; // Variable to indicate if the player is dead
    public static PlayerHealth instance; // Static instance of the PlayerHealth class for easy access
    public bool playerShield = false; // Variable to indicate if the player's shield is active
    [HideInInspector] public int health; // The player's current health (initial value set in SetPlayerHealth)
    public ParticleSystem playerDeathFX; // Particle system effect for player death
    public AudioClip playerDeathSound; // Audio clip to play when the player dies

    void Awake()
    {
        // Assigns the current instance of this script to the static instance variable
        instance = this;
    }

    // Method to set the player's health based on the selected difficulty mode
    public void SetPlayerHealth()
    {
        // Sets the player's health based on the difficulty mode stored in PlayerPrefs
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            health = 60; // Sets health to 60 in easy mode
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            health = 50; // Sets health to 50 in normal mode
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
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
                GameManager.instance.playerHealthText.text = "Player 1 Health: " + health.ToString();
            }
            else if (gameObject.name == "PlayerMultiplayer2")
            {
                GameManager.instance.playerHealthText2.text = "Player 2 Health: " + health.ToString();
            }

            // If health reaches 0 or less and the player is not already dead, calls the Die method
            if (health <= 0 && !playerDeath)
            {
                // Plays the player death sound at the SpawnManager's position
                AudioSource.PlayClipAtPoint(playerDeathSound, SpawnManager.instance.transform.position);

                // Plays the particle effect for player death
                playerDeathFX.Play();

                // Calls the Die method after a delay of 0.3 seconds
                Invoke(nameof(Die), 0.3f);
            }
        }
    }

    // Method to handle the player's death
    void Die()
    {
        // Removes the player from the list of active players in the level
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Remove(gameObject);

        // Loads the next level or handles level restart in the SpawnManager
        SpawnManager.instance.GameOver();

        // Sets playerDeath to true to prevent multiple calls to the Die method
        playerDeath = true;

        // Destroys the player game object
        Destroy(gameObject);

        // Resets the main camera's viewport if it is not null
        if (Camera.main != null)
        {
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }

        // Assigns a new target for alien enemies in multiplayer mode
        SpawnManager.instance.AssignAlienTargetForMultiplayer();
    }
}
