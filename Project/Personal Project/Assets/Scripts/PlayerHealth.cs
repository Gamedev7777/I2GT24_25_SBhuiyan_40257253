using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance; // Created an instance of the player health class to access the player health class from another class

    void Awake()
    {
        instance = this; // It means this particular instance of the script
    }

    private int _health = 50; // Set the initial health

    public void TakeDamage(int damage)
    {
        {
            _health -= damage; // reduces health by the value assigned to the damage variable for each time the player is shot by alien laser
            
            if (_health <= 0)
            {
                Die(); // Call a method to handle the player's death
            }
        }
    }

    void Die()
    {
        // Handle player death
        Destroy(gameObject);
    }
}