using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    void Awake()
    {
        instance = this;
    }

    private int _health = 50; // Set the initial health

    public void TakeDamage(int damage)
    {
        {
            _health -= damage;
            Debug.Log($"Player took {damage} damage! Current health: {_health}"); // Log damage taken

            if (_health <= 0)
            {
                Die(); // Call a method to handle the player's death
            }
        }
    }

    void Die()
    {
        // Handle player death
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }
}