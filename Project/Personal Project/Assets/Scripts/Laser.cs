using UnityEngine;
public class Laser : MonoBehaviour

{
    // This method handles what happens when the laser hits various objects
    private void OnTriggerEnter(Collider other)
    {
        // Check if the laser hits the player
        if (other.CompareTag("Player"))
        {
            // Reduce the player's health by one
            PlayerHealth.Instance.TakeDamage(1);
            // After hitting the player destroy the laser
            Destroy(gameObject);
        }
        // Check if the laser has hit a fixed or moving wall
        else if ((other.CompareTag("FixedWall")) || other.CompareTag("MovingWall"))
        {
            //Destroy the laser when it hits a wall so it does not pass through it    
            Destroy(gameObject);
        }
        // Check if the laser hits a power-up
        else if ((other.CompareTag("PowerUp1") || other.CompareTag("PowerUp2")) ||

        other.CompareTag("PowerUp3"))
        {
            // When the laser hits a PowerUp destroy the laser
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the laser after 1 second to prevent clutter
        Destroy(gameObject, 1.0f);
    }
}