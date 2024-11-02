using UnityEngine;

public class Bullet : MonoBehaviour
{
    // This method handles what happens when the bullet hits various objects
    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet hits the alien
        if (other.CompareTag("Alien"))
        {
            // Reduce the alien's health by one
            other.gameObject.GetComponent<AlienHealth>().TakeDamage(1);
            // After hitting the alien destroy the bullet
            Destroy(gameObject);
        }
        // Check if the bullet has hit a fixed or moving wall
        else if ((other.CompareTag("FixedWall")) || other.CompareTag("MovingWall"))
        {
            //Destroy the bullet when it hits a wall so it does not pass through it    
            Destroy(gameObject);
        }
        // Check if the bullet hits a power-up
        else if ((other.CompareTag("PowerUp1") || other.CompareTag("PowerUp2")) ||

                 other.CompareTag("PowerUp3"))
        {
            // When the laser hits a PowerUp destroy the bullet
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the bullet after 1 second to prevent clutter
        Destroy(gameObject, 1.0f);
    }
}