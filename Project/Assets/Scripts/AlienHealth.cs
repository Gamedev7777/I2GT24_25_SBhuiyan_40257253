using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    // public variables
    public Transform healthBar; // Health bar variable for alien
    public int health = 10; // Initial health of the alien, configurable in the inspector
    public ParticleSystem alienDeathFX; // Particle effect to play on alien death
    public AudioClip alienDeathSound; // Audio clip to play on alien death

    // private variable
    private bool _alienDeath; // Tracks if the alien is already dead, to prevent duplicate death actions
    private float _maxHealth; // Maximum health amount
    private readonly float _yAxisScale = 0.1031f; // Sets the Y axis scale of the health bar
    private readonly int _zAxisScale = 1; // Sets the Z axis scale of the health bar

    // Method to set the alien's health based on the current game mode
    public void SetAlienHealth()
    {
        // Checks the game mode from PlayerPrefs and sets health accordingly
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            if (gameObject.CompareTag("Alien1"))
            {
                health = 7; // Sets health for Alien1 in easy mode
            }
            else if (gameObject.CompareTag("Alien2"))
            {
                health = 15; // Sets health for Alien2 in easy mode
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            if (gameObject.CompareTag("Alien1"))
            {
                health = 10; // Sets health for Alien1 in normal mode
            }
            else if (gameObject.CompareTag("Alien2"))
            {
                health = 20; // Sets health for Alien2 in normal mode
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
            if (gameObject.CompareTag("Alien1"))
            {
                health = 15; // Sets health for Alien1 in hard mode
            }
            else if (gameObject.CompareTag("Alien2"))
            {
                health = 25; // Sets health for Alien2 in hard mode
            }
        }

        _maxHealth = health; // Initialises the maximum health value
    }

    // Method to reduce health when alien takes damage
    public void TakeDamage(int damage)
    {
        // Reduces health by the damage amount
        health -= damage;

        UpdateHealthBar(); // Updates the health bar display
        
        // If health is zero or less and the alien is not already dead
        if (health <= 0 && !_alienDeath)
        {
            // Increases player's score by an amount based on game mode
            UpdateScore();

            // Plays the death sound at the SpawnManager's position
            AudioSource.PlayClipAtPoint(alienDeathSound, SpawnManager.instance.transform.position);

            // Plays the death particle effect
            alienDeathFX.Play();

            // Calls the Die method to handle alien's death after a slight delay
            Invoke(nameof(Die), 0.3f);
        }
    }

    // Method to update the player's score based on game mode
    private static void UpdateScore()
    {
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100); // Adds 100 points to the score
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 200); // Adds 200 points to the score
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 300); // Adds 300 points to the score
        }
        
        // Updates the score text in the UI
        GameManager.instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score");
    }

    // Method to update the alien's health bar
    private void UpdateHealthBar()
    {
        // Calculates the health percentage and updates the scale of the health bar
        var healthPercentage = health / _maxHealth;
        healthBar.localScale = new Vector3(healthPercentage, _yAxisScale, _zAxisScale);
    }

    // Method to handle the death of the alien
    private void Die()
    {
        // Sets the alien's death status to true to avoid duplicate death processing
        _alienDeath = true;

        // Removes the alien from the list of active aliens in the SpawnManager
        SpawnManager.instance.alienList.Remove(gameObject);

        // Calls the LevelComplete method from the SpawnManager to check if the level is complete
        SpawnManager.instance.LevelComplete();

        // Destroys the alien game object
        Destroy(gameObject);
    }
}
