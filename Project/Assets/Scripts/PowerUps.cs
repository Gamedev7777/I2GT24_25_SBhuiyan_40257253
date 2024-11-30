using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // public variable
    // AudioClip to be played when a power-up is collected by the player
    public AudioClip powerUpSound;

    void Start()
    {
        // Initialises the Speed Power-Up status to zero (inactive) at the beginning of the game
        PlayerPrefs.SetInt("SpeedPowerUp", 0);
    }

    // Triggers event handler to process power-ups when a collider enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object is the Player
        if (other.gameObject.tag == "Player")
        {
            // Plays the power-up sound effect at the specified location (the position of the SpawnManager)
            AudioSource.PlayClipAtPoint(powerUpSound, SpawnManager.instance.transform.position);

            // Processes Health Power-Up if the current gameObject is tagged as PowerUpHealth
            if (gameObject.tag == "PowerUpHealth")
            {
                ProcessHealthPowerUp();
            }
            // Processes Speed Power-Up if the current gameObject is tagged as PowerUpSpeed
            else if (gameObject.tag == "PowerUpSpeed")
            {
                ProcessSpeedPowerUp();
            }
            // Processes Shield Power-Up if the current gameObject is tagged as PowerUpShield
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
            // Single-player mode: increases Player 1's health by 20 and updates User Interface
            var player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
            player1.health += 20;

            // Updates Player 1's health text in the User Interface
            GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            PlayerHealth player1;
            PlayerHealth player2;
            
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                // Multiplayer mode: increase health for both Player 1 and Player 2 by 20 and updates User Interface
                player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
                player2 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>();
                
                player1.health += 20;
                player2.health += 20;
                
                // Updates both Player 1 and Player 2's health text in the User Interface
                GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
                GameManager.instance.playerHealthText2.text = "Player 2 Health: " + player2.health;
            }
            else
            {
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
                    player1.health += 20;
                    GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
                }
                else if(SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    player2 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
                    player2.health += 20;
                    GameManager.instance.playerHealthText.text = "Player 2 Health: " + player2.health;
                }
            }
        }

        // Destroys the Health Power-Up object after applying its effect
        Destroy(gameObject);
    }

    // Handles the logic for the Speed Power-Up
    void ProcessSpeedPowerUp()
    {
        // Activates the Speed Power-Up User Interface indicator
        GameManager.instance.playerSpeedText.SetActive(true);
        
        // Marks the Speed Power-Up as activated in PlayerPrefs
        PlayerPrefs.SetInt("SpeedPowerUp", 1);

        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: doubles Player 1's speed
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                .GetComponent<PlayerController>().playerSpeed *= 2.0f;
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                // Multiplayer mode: doubles speed for both Player 1 and Player 2
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                    .GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1]
                    .GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
            }
            else
            {
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    // Multiplayer mode: doubles speed for both Player 1 and Player 2
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                        .GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
                    
                }
                else if(SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0]
                        .GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
                }
            }
        }

        // Destroys the Speed Power-Up object after applying its effect
        Destroy(gameObject);
    }

    // Handles the logic for the Shield Power-Up
    void ProcessShieldPowerUp()
    {
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: activates Player 1's shield
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                .playerShield = true;

            // Activates both shield visuals for Player 1
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>()
                .remyShield.SetActive(true);
            SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>()
                .claireShield.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                // Multiplayer mode: activates shields for both Player 1 and Player 2
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                    .playerShield = true;
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>()
                    .playerShield = true;

                // Activates shield visual for Player 1
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>()
                    .remyShield.SetActive(true);

                // Activates shield visual for Player 2
                SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerMultiplayer2>()
                    .claireShield.SetActive(true);
            }
            else
            {
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    // Multiplayer mode: activates shields for both Player 1 and Player 2
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .playerShield = true;
                    

                    // Activates shield visual for Player 1
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>()
                        .remyShield.SetActive(true);

                    
                }
                else if(SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    // Multiplayer mode: activates shields for both Player 1 and Player 2
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>()
                        .playerShield = true;
                    // Activates shield visual for Player 2
                    SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer2>()
                        .claireShield.SetActive(true);
                }
            }
        }

        // Activates the Shield Power-Up User Interface indicator
        GameManager.instance.playerShieldText.SetActive(true);

        // Notifies SpawnManager to process the shield effect for all players
        SpawnManager.instance.ProcessPlayerShield();

        // Destroys the Shield Power-Up object after applying its effect
        Destroy(gameObject);
    }
}
