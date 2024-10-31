using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    
    public static AlienHealth instance;
private bool _alienDeath = false;
    void Awake()
    {
        instance = this;
    }
        
    private int _health = 10; // Set the initial health
    
    public void TakeDamage(int damage)
    {
        // if (!_isShielded) // Only take damage if not shielded
        {
            _health -= damage;
            
            if (_health <= 0 && !_alienDeath)
            {
                Die(); // Call a method to handle the player's death
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Die()
    {
        // Handle player death
        _alienDeath = true;
        Destroy(gameObject);
    }
}
