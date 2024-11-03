using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public bool playerDeath = false;
    public static PlayerHealth Instance; // Created an instance of the player health class to access the player health class from another class

    void Awake()
    {
        Instance = this; // It means this particular instance of the script
    }

    public int health = 50; // Set the initial health

    public void TakeDamage(int damage)
    {
        {
            health -= damage; // reduces health by the value assigned to the damage variable for each time the player is shot by alien laser
            
            GameManager.Instance.playerHealthText.text = "Player Health: " + health.ToString();
            if (health <= 0 && !playerDeath)
            {
                Die(); // Call a method to handle the player's death
            }
        }
    }

    void Die()
    {
        // Handle player death
        Debug.Log("Player Death");
        SpawnManager.Instance.LevelLoad();
        playerDeath = true;
        Destroy(gameObject);
    }
    
}