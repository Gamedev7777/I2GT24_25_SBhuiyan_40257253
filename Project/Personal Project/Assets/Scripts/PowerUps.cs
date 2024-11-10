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
                // Checks if the game is in single-player mode
                if (PlayerPrefs.GetInt("Controller", 0) == 0)
                {
                    // Increases player 1's health by 20
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .health += 20;

                    // Updates the User Interface to show player 1's new health value
                    GameManager.Instance.playerHealthText.text = "Player 1 Health: " + SpawnManager.Instance
                        ._levelSpawned
                        .GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().health.ToString();
                }
                // Checks if the game is in multiplayer mode
                else if (PlayerPrefs.GetInt("Controller", 0) == 1)
                {
                    // Increases player 1's health by 20
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .health += 20;

                    // Increases player 2's health by 20
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                        .health += 20;

                    // Updates the User Interface to show player 1's new health value
                    GameManager.Instance.playerHealthText.text = "Player 1 Health: " + SpawnManager.Instance
                        ._levelSpawned
                        .GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().health.ToString();

                    // Updates the User Interface to show player 2's new health value
                    GameManager.Instance.playerHealthText2.text = "Player 2 Health: " + SpawnManager.Instance
                        ._levelSpawned
                        .GetComponent<Levels>().player[1].GetComponent<PlayerHealth>().health.ToString();
                }

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
                
                // Doubles the player's speed in single-player mode
                if (PlayerPrefs.GetInt("Controller", 0) == 0)
                {
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                        .GetComponent<PlayerController>().playerSpeed *= 2.0f;
                }
                // Doubles both players' speed in multiplayer mode
                else if (PlayerPrefs.GetInt("Controller", 0) == 1)
                {
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0]
                        .GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1]
                        .GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
                }

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
                // Activates the shield for player 1 in single-player mode
                if (PlayerPrefs.GetInt("Controller", 0) == 0)
                {
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .playerShield = true;
                }
                // Activates the shield for both players in multiplayer mode
                else if (PlayerPrefs.GetInt("Controller", 0) == 1)
                {
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .playerShield = true;
                    SpawnManager.Instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                        .playerShield = true;
                }

                // Activates the User Interface element indicating that the shield power-up is active
                GameManager.Instance.playerShieldText.SetActive(true);

                // Notifies the SpawnManager to process the shield effect
                SpawnManager.Instance.ProcessPlayerShield();
                
                // Destroys the Shield Power-Up object after use
                Destroy(gameObject);
            }
        }
    }
}
