using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    // public variable
    // Singleton instance of the AlienHealth class
    public static AlienHealth Instance;

    // private variables
    private bool _alienDeath = false; // Tracks if the alien is already dead
    public int health = 10; // Initial health of the alien
    public ParticleSystem alienDeathFX;
    public AudioClip alienDeathSound;
    void Awake()
    {
        // Assigns the instance of this script to the static Instance variable
        Instance = this;
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
    }

    // Method to reduce health when alien takes damage
    public void TakeDamage(int damage)
    {
        // Reduces health by the damage amount
        //health -= damage;
        health -= 10;
    
        // If health is zero or less and the alien is not already dead
        if (health <= 0 && !_alienDeath)
        {
            // Increases player's score by 100 points
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);

            // Updates the score text in the GameManager
            GameManager.Instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score").ToString();
            AudioSource.PlayClipAtPoint(alienDeathSound, SpawnManager.Instance.transform.position);
            alienDeathFX.Play();
            // Calls the Die method to handle alien's death
            Invoke(nameof(Die),0.3f);
        }
    }

    // Method to handle the death of the alien
    void Die()
    {
        // Sets the alien's death status to true
        _alienDeath = true;

        // Removes the alien from the list of active aliens in the SpawnManager
        SpawnManager.Instance.alienList.Remove(gameObject);

        // Calls the LevelComplete method from the SpawnManager to check if the level is complete
        SpawnManager.Instance.LevelComplete();

        // Destroys the alien game object
        Destroy(gameObject);
    }
}
