using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        // Checks if the bullet hits the alien
        if (other.gameObject.CompareTag("Alien1") || other.gameObject.CompareTag("Alien2"))
        {
            // Reduces the alien's health by one by calling the TakeDamage method in the AlienHealth component
            other.gameObject.GetComponentInParent<AlienHealth>().TakeDamage(1);
            Debug.Log("one health point deducted");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Destroy(gameObject, 0.5f);
    }
    
}