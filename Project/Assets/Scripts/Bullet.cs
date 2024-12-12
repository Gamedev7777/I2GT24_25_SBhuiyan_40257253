using UnityEngine;

public class Bullet : MonoBehaviour
{
    // This method is called when the bullet collides with another object
    private void OnCollisionEnter(Collision other)
    {
        // Checks if the bullet hits an alien by comparing tags
        if (other.gameObject.CompareTag("Alien1") || other.gameObject.CompareTag("Alien2"))
        {
            // Checks the difficulty mode stored in PlayerPrefs and applies appropriate damage
            if (PlayerPrefs.GetInt("Mode", 0) == 0) // Easy mode
            {
                // Reduces the alien's health by 5 in easy mode
                DealDamage(other, 5);
            }
            else if (PlayerPrefs.GetInt("Mode", 0) == 1) // Normal mode
            {
                // Reduces the alien's health by 3 in normal mode
                DealDamage(other, 3);
            }
            else if (PlayerPrefs.GetInt("Mode", 0) == 2) // Hard mode
            {
                // Reduces the alien's health by 1 in hard mode
                DealDamage(other, 1);
            }

            // Destroys the bullet after it collides with the alien
            Destroy(gameObject);
        }
    }

    private static void DealDamage(Collision other, int damage)
    {
        other.gameObject.GetComponentInParent<AlienHealth>().TakeDamage(damage);
    }

    private void Update()
    {
        // Destroys the bullet automatically after 0.5 seconds if it doesn't collide with an alien
        Destroy(gameObject, 0.5f);
    }
}