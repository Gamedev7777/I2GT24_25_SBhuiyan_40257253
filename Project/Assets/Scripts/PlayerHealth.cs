using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Public variables
    public bool playerDeath; // Variable to indicate if the player is dead
    public bool playerShield; // Variable to indicate if the player's shield is active
    [HideInInspector] public int health; // The player's current health (initial value set in SetPlayerHealth)
    public ParticleSystem playerDeathFX; // Particle system effect for player death
    public AudioClip playerDeathSound; // Audio clip to play when the player dies
    public Transform healthBar; // Gets the transform component from the inspector window of the health bar
    public float maxHealth; // Maximum health of the player
    
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

        maxHealth = health;
    }

    // Method to handle when the player takes damage
    public void TakeDamage(int damage)
    {
        // Only takes damage if the player's shield is not active
        if (playerShield == false)
        {
            if (health <= 0)
            {
                health = 0;
            }
            else
            {
                health -= damage;
            }

            UpdateHealthBar();
            
            UpdatePlayerHealthText();

            // If health reaches 0 or less and the player is not already dead
            if (health <= 0 && !playerDeath)
            {
                // Sets playerDeath to true to prevent multiple calls to the Die method
                playerDeath = true;
                
                PlayerDeathSound();
                
                TurnOffPlayer();

                // Plays the particle effect for player death
                playerDeathFX.Play();
                
                // Calls the Die method after a delay of 0.7 seconds
                Invoke(nameof(Die), 0.7f);
            }
        }
    }

    private void TurnOffPlayer()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0) // Single player mode
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void PlayerDeathSound()
    {
        AudioSource.PlayClipAtPoint(playerDeathSound, SpawnManager.instance.transform.position);
    }

    private void UpdatePlayerHealthText()
    {
        // Updates the health display in the User Interface based on the player's name
        if (gameObject.name == "Player" || gameObject.name == "PlayerMultiplayer1")
        {
            GameManager.instance.playerHealthText.text = "Player 1 Health: " + health.ToString();
        }
        else if (gameObject.name == "PlayerMultiplayer2")
        {
            GameManager.instance.playerHealthText2.text = "Player 2 Health: " + health.ToString();
        }
    }

    private void UpdateHealthBar()
    {
        var healthPercentage = health / maxHealth; // Calculating how much health is left and storing it

        healthBar.localScale = new Vector3(healthPercentage, 0.1031f, 1); // Using the above to scale the health bar
    }

    // Method to handle the player's death
    private void Die()
    {
        // Removes the player from the list of active players in the level
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Remove(gameObject);

        // Restarts the level using the Spawn Manager script
        SpawnManager.instance.GameOver();

        // Destroys the player game object
        Destroy(gameObject);

        ResetCameraToFullscreenView();

        // Assigns a new target for alien enemies in multiplayer mode
        SpawnManager.instance.AssignAlienTargetForMultiplayer();
    }

    private static void ResetCameraToFullscreenView()
    {
        // Resets the main camera's viewport if it is not null
        if (Camera.main != null)
        {
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }
    }
}