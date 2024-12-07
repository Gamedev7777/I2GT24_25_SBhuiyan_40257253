using UnityEngine;

public class Laser : MonoBehaviour
{
    // public variable
    public int laserDamage; // Damage value of the laser

    // This method handles what happens when the laser hits various objects
    void OnTriggerEnter(Collider other)
    {
        // Checks if the laser hits the player
        if (other.CompareTag("Player"))
        {
            // Reduces the player's health by calling the TakeDamage method in the PlayerHealth component
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(laserDamage);

            // After hitting the player, destroys the laser
            Destroy(gameObject);
        }

        // Checks if the laser has hit a power-up (speed, shield, or health) or fixed wall
        else if (other.CompareTag("PowerUpSpeed") || other.CompareTag("PowerUpShield") ||
                 other.CompareTag("PowerUpHealth") || other.CompareTag("FixedWall"))
        {
            // When the laser hits a power-up, destroys the laser
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Destroys the laser after 1 second to prevent cluttering the scene and keeps the game optimised
        Destroy(gameObject, 1.0f);
    }
}