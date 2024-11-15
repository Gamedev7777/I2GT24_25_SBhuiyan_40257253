using UnityEngine;

public class PowerUps : MonoBehaviour
{
    void Start()
    {
        // Initializes the Speed Power-Up status to zero at the beginning of the game
        PlayerPrefs.SetInt("SpeedPowerUp", 0);
    }

    // Trigger event handler to process power-ups when a collider enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object is the Player
        if (other.gameObject.tag == "Player")
        {
            // Process Health Power-Up
            if (gameObject.tag == "PowerUpHealth")
            {
                ProcessHealthPowerUp();
            }

            // Process Speed Power-Up
            else if (gameObject.tag == "PowerUpSpeed")
            {
                ProcessSpeedPowerUp();
            }

            // Process Shield Power-Up
            else if (gameObject.tag == "PowerUpShield")
            {
                ProcessShieldPowerUp();
            }
        }
    }

    // Handles the logic for the Health Power-Up
    void ProcessHealthPowerUp()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: increase Player 1's health by 20 and update UI
            var player1 = SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
            player1.health += 20;

            GameManager.Instance.playerHealthText.text = "Player 1 Health: " + player1.health.ToString();
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            // Multiplayer mode: increase health for both Player 1 and Player 2 by 20 and update UI
            var player1 = SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
            var player2 = SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>();

            player1.health += 20;
            player2.health += 20;

            GameManager.Instance.playerHealthText.text = "Player 1 Health: " + player1.health.ToString();
            GameManager.Instance.playerHealthText2.text = "Player 2 Health: " + player2.health.ToString();
        }

        // Destroy the Health Power-Up object after applying its effect
        Destroy(gameObject);
    }

    // Handles the logic for the Speed Power-Up
    void ProcessSpeedPowerUp()
    {
        // Activate the Speed Power-Up UI indicator
        GameManager.Instance.playerSpeedText.SetActive(true);
        PlayerPrefs.SetInt("SpeedPowerUp", 1); // Mark the Speed Power-Up as activated

        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: double Player 1's speed
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                .GetComponent<PlayerController>().playerSpeed *= 2.0f;
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            // Multiplayer mode: double speed for both Player 1 and Player 2
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                .GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1]
                .GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
        }

        // Destroy the Speed Power-Up object after applying its effect
        Destroy(gameObject);
    }

    // Handles the logic for the Shield Power-Up
    void ProcessShieldPowerUp()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: activate Player 1's shield
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .playerShield = true;
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            // Multiplayer mode: activate shields for both Player 1 and Player 2
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .playerShield = true;
            SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                .playerShield = true;
        }

        // Activate the Shield Power-Up UI indicator
        GameManager.Instance.playerShieldText.SetActive(true);

        // Notify SpawnManager to process the shield effect
        SpawnManager.Instance.ProcessPlayerShield();

        // Destroy the Shield Power-Up object after applying its effect
        Destroy(gameObject);
    }
}
