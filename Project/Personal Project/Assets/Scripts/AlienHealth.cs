using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    // public variable
    // Singleton instance of the AlienHealth class
    public static AlienHealth Instance;

    // private variables
    private bool _alienDeath = false; // Tracks if the alien is already dead
    public int health = 10; // Initial health of the alien

    void Awake()
    {
        // Assigns the instance of this script to the static Instance variable
        Instance = this;
    }

    public void SetAlienHealth()
    {
        if (PlayerPrefs.GetInt("Mode", 0) == 0)
        {
            if (gameObject.tag == "Alien1")
            {
                health = 7;
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 15;
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 1)
        {
            if (gameObject.tag == "Alien1")
            {
                health = 10;
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 20;
            }
        }
        else if (PlayerPrefs.GetInt("Mode", 0) == 2)
        {
            if (gameObject.tag == "Alien1")
            {
                health = 15;
            }
            else if (gameObject.tag == "Alien2")
            {
                health = 25;
            }
        }
    }

    // Method to reduce health when alien takes damage
    public void TakeDamage(int damage)
    {
        // Reduces health by the damage amount
        health -= damage;

        // If health is zero or less and the alien is not already dead
        if (health <= 0 && !_alienDeath)
        {
            // Increases player's score by 100 points
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);

            // Updates the score text in the GameManager
            GameManager.Instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score").ToString();

            // Call the Die method to handle alien's death
            Die();
        }
    }

    // Method to handle the death of the alien
    void Die()
    {
        // Sets the alien's death status to true
        _alienDeath = true;

        // Logs a message indicating the alien has died
        Debug.Log("Aliens are dead");

        // Removes the alien from the list of active aliens in the SpawnManager
        SpawnManager.Instance.alienList.Remove(gameObject);

        // Calls the LevelComplete method from the SpawnManager to check if the level is complete
        SpawnManager.Instance.LevelComplete();

        // Destroys the alien game object
        Destroy(gameObject);
    }
}