using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // Method triggered when another collider enters the trigger collider attached to the power-up object
    void OnTriggerEnter(Collider other)
    {
        // Checks if the current gameObject is a Health Power-Up
        if (gameObject.tag == "PowerUpHealth")
        {
            // Checks if the colliding object is the Player
            if (other.gameObject.tag == "Player")
            {
                // Increases player's health by 20 points
                PlayerHealth.Instance.health += 20;
                // Updates the player health text User Interface to reflect the new health value
                GameManager.Instance.playerHealthText.text = "Player Health: " + PlayerHealth.Instance.health.ToString();
                // Destroys the Health Power-Up object after use
                Destroy(gameObject);
            }   
        }

        // Checks if the current gameObject is a Speed Power-Up
        if (gameObject.tag == "PowerUpSpeed")
        {
            // Checks if the colliding object is the Player
            if (other.gameObject.tag == "Player")
            {
                // Activates the User Interface element indicating that the speed power-up is active
                GameManager.Instance.playerSpeedText.SetActive(true);
                // Doubles the player's speed
                SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player.GetComponent<PlayerController>().playerSpeed *= 2.0f;
                // Destroys the Speed Power-Up object after use
                Destroy(gameObject);
            }
        }

        // Checks if the current gameObject is a Shield Power-Up
        if (gameObject.tag == "PowerUpShield")
        {
            // Checks if the colliding object is the Player
            if (other.gameObject.tag == "Player")
            {
                // Activates the User Interface element indicating that the shield power-up is active
                GameManager.Instance.playerShieldText.SetActive(true);
                // Sets player's shield status to true
                PlayerHealth.Instance.playerShield = true;
                // Notifies the SpawnManager to process the shield effect
                SpawnManager.Instance.ProcessPlayerShield();
                // Destroys the Shield Power-Up object after use
                Destroy(gameObject);
            }
        }
    }
}