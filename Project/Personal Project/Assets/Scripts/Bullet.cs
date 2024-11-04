using UnityEngine;

public class Bullet : MonoBehaviour
{
    // This method handles what happens when the bullet collides with various objects
    void OnTriggerEnter(Collider other)
    {
        // Checks if the bullet hits the alien
        if (other.CompareTag("Alien"))
        {
            // Reduces the alien's health by one by calling the TakeDamage method in the AlienHealth component
            other.gameObject.GetComponent<AlienHealth>().TakeDamage(1);
            // Destroys the bullet after it hits the alien to remove it from the scene
            Destroy(gameObject);
        }
        // Checks if the bullet hits a fixed or moving wall
        else if ((other.CompareTag("FixedWall")) || other.CompareTag("MovingWall"))
        {
            // Destroys the bullet when it hits a wall (either fixed or moving) to prevent it from passing through
            Destroy(gameObject);
        }
        // Checks if the bullet hits a power-up of any type (speed, shield, or health)
        else if ((other.CompareTag("PowerUpSpeed") || other.CompareTag("PowerUpShield")) ||
                 other.CompareTag("PowerUpHealth"))
        {
            // Destroys the bullet if it hits any type of power-up to prevent unintended interactions
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Destroys the bullet after 1 second to prevent cluttering the scene and keeps the game optimised
        Destroy(gameObject, 1.0f);
    }
}