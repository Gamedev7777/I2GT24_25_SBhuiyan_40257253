using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    // public variables
    // Singleton instance of the AlienHealth class (accessible globally)
    public static AlienHealth instance;

    public Transform healthBar;
    // Initial health of the alien, configurable in the inspector
    public int health = 10;

    private float maxHealth;
    // Particle effect to play on alien death
    public ParticleSystem alienDeathFX;

    // Audio clip to play on alien death
    public AudioClip alienDeathSound;
    
    // private variable
    // Tracks if the alien is already dead, to prevent duplicate death actions
    private bool _alienDeath = false;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Assigns the instance of this script to the static instance variable
        instance = this;
    }

    // Method to set the alien's health based on the current game mode
    public void SetAlienHealth()
    {
        // Checks the game mode from PlayerPrefs and sets health accordingly
        if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
        {
            if (gameObject.tag == "Alien1")
            {
                health = 7; // Sets health for Alien1 in easy mode
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 15; // Sets health for Alien2 in easy mode
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
        {
            if (gameObject.tag == "Alien1")
            {
                health = 10; // Sets health for Alien1 in normal mode
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 20; // Sets health for Alien2 in normal mode
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
        {
            if (gameObject.tag == "Alien1")
            {
                health = 15; // Sets health for Alien1 in hard mode
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 25; // Sets health for Alien2 in hard mode
            }
        }

        maxHealth = health;
    }

    // Method to reduce health when alien takes damage
    public void TakeDamage(int damage)
    {
        // Reduces health by the damage amount
        health -= damage;
        
        float healthPercentage = health / maxHealth;
        healthBar.localScale = new Vector3(healthPercentage, 0.1031f, 1);
        // If health is zero or less and the alien is not already dead
        if (health <= 0 && !_alienDeath)
        {
            // Increases player's score by 100 points
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);

            // Updates the score text in the GameManager
            GameManager.instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score").ToString();

            // Plays the death sound at the SpawnManager's position
            AudioSource.PlayClipAtPoint(alienDeathSound, SpawnManager.instance.transform.position);

            // Plays the death particle effect
            alienDeathFX.Play();

            // Calls the Die method to handle alien's death after a slight delay
            Invoke(nameof(Die), 0.3f);
        }
    }

    // Method to handle the death of the alien
    void Die()
    {
        // Sets the alien's death status to true to avoid duplicate death processing
        _alienDeath = true;

        // Removes the alien from the list of active aliens in the SpawnManager
        SpawnManager.instance.alienList.Remove(gameObject);

        // Calls the LevelComplete method from the SpawnManager to check if the level is complete
        SpawnManager.instance.LevelComplete();

        // Destroys the alien game object, removing it from the game
        Destroy(gameObject);
    }
}
