using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    public static AlienHealth Instance;
    private bool _alienDeath = false;

    void Awake()
    {
        Instance = this;
    }

    private int _health = 10; // Set the initial health

    public void TakeDamage(int damage)
    {
        
            _health -= damage;

            if (_health <= 0 && !_alienDeath)
            {
                Die(); // Call a method to handle the player's death
                Debug.Log("Die");
            }
            Debug.Log(_health);
        
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
        // Handle alien death
        _alienDeath = true;
        //SpawnManager.Instance.alienList.Remove(gameObject);
        // SpawnManager.Instance.LevelComplete();
        Debug.Log(gameObject.name);
        Destroy(gameObject);
    }
}