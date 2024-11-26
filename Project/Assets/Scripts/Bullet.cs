using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        // Checks if the bullet hits the alien
        if (other.gameObject.CompareTag("Alien1") || other.gameObject.CompareTag("Alien2"))
        {
            if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
            {
                other.gameObject.GetComponentInParent<AlienHealth>().TakeDamage(10);
            }
            else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
            {
                other.gameObject.GetComponentInParent<AlienHealth>().TakeDamage(5);
            }
            else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
            {
                other.gameObject.GetComponentInParent<AlienHealth>().TakeDamage(2);
            }
            // Reduces the alien's health by one by calling the TakeDamage method in the AlienHealth component
            
            
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Destroy(gameObject, 0.5f);
    }
    
}