using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // AudioClip to be played when a power-up is collected by the player
    public AudioClip powerUpSound;

    private void Start()
    {
        // Initializes the Speed Power-Up status to zero (inactive) at the beginning of the game
        PlayerPrefs.SetInt("SpeedPowerUp", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object is the Player
        if (other.gameObject.CompareTag("Player"))
        {
            // Plays the power-up sound effect
            PlayPowerUpSound();

            // Processes the appropriate power-up based on the gameObject's tag
            if (gameObject.CompareTag("PowerUpHealth"))
            {
                ProcessHealthPowerUp();
            }
            else if (gameObject.CompareTag("PowerUpSpeed"))
            {
                ProcessSpeedPowerUp();
            }
            else if (gameObject.CompareTag("PowerUpShield"))
            {
                ProcessShieldPowerUp();
            }
        }
    }

    private void PlayPowerUpSound()
    {
        // Plays the power-up sound effect at the specified location (SpawnManager's position)
        AudioSource.PlayClipAtPoint(powerUpSound, SpawnManager.instance.transform.position);
    }

    private void ProcessHealthPowerUp()
    {
        // Handles health increase logic based on the current game mode (single-player or multiplayer)
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            // Single-player mode: updates Player 1's health and health bar
            var player1 = UpdatePlayerHealthSingleplayer();
            UpdateHealthBarSingleplayer(player1);
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            // Multiplayer mode: checks for multiple players and updates their health
            PlayerHealth player1, player2;
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                player1 = UpdatePlayer1HealthMultiplayer();
                player2 = UpdatePlayer2HealthMultiplayer();
                UpdateHealthBarsMultiplayer(player1, player2);
            }
            else
            {
                // Updates health for the alive player in multiplayer mode
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    player1 = UpdateAlivePlayer1HealthMultiPlayerMode();
                    UpdateMultiplayer1HealthBar(player1);
                }
                else if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    player2 = UpdateAlivePlayer2HealthMultiPlayerMode();
                    UpdateMultiplayer2HealthBar(player2);
                }
            }
        }

        // Destroys the Health Power-Up object after applying its effect
        Destroy(gameObject);
    }

    private static void UpdateMultiplayer2HealthBar(PlayerHealth player2)
    {
        // Updates Player 2's health bar in multiplayer mode
        var healthPercentage = player2.health / player2.maxHealth;
        player2.healthBar.localScale = new Vector3(healthPercentage, 0.1031f, 1);
    }

    private static PlayerHealth UpdateAlivePlayer2HealthMultiPlayerMode()
    {
        // Increases Player 2's health in multiplayer mode
        var player2 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
        player2.health += 20;
        GameManager.instance.playerHealthText.text = "Player 2 Health: " + player2.health;
        return player2;
    }

    private static void UpdateMultiplayer1HealthBar(PlayerHealth player1)
    {
        // Updates Player 1's health bar in multiplayer mode
        var healthPercentage = player1.health / player1.maxHealth;
        player1.healthBar.localScale = new Vector3(healthPercentage, 0.1031f, 1);
    }

    private static PlayerHealth UpdateAlivePlayer1HealthMultiPlayerMode()
    {
        // Increases Player 1's health in multiplayer mode
        var player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
        player1.health += 20;
        GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
        return player1;
    }

    private static void UpdateHealthBarsMultiplayer(PlayerHealth player1, PlayerHealth player2)
    {
        // Updates health bars for both players in multiplayer mode
        var healthPercentage1 = player1.health / player1.maxHealth;
        player1.healthBar.localScale = new Vector3(healthPercentage1, 0.1031f, 1);

        var healthPercentage2 = player2.health / player2.maxHealth;
        player2.healthBar.localScale = new Vector3(healthPercentage2, 0.1031f, 1);
    }

    private static PlayerHealth UpdatePlayer2HealthMultiplayer()
    {
        // Increases Player 2's health in multiplayer mode
        var player2 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>();
        player2.health += 20;
        GameManager.instance.playerHealthText2.text = "Player 2 Health: " + player2.health;
        return player2;
    }

    private static PlayerHealth UpdatePlayer1HealthMultiplayer()
    {
        // Increases Player 1's health in multiplayer mode
        var player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
        player1.health += 20;
        GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
        return player1;
    }

    private static void UpdateHealthBarSingleplayer(PlayerHealth player1)
    {
        // Updates Player 1's health bar in single-player mode
        var healthPercentage = player1.health / player1.maxHealth;
        player1.healthBar.localScale = new Vector3(healthPercentage, 0.1031f, 1);
    }

    private static PlayerHealth UpdatePlayerHealthSingleplayer()
    {
        // Single-player mode: increases Player 1's health and updates User Interface
        var player1 = SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>();
        player1.health += 20;
        GameManager.instance.playerHealthText.text = "Player 1 Health: " + player1.health;
        return player1;
    }

    private void ProcessSpeedPowerUp()
    {
        // Activates the Speed Power-Up User Interface indicator
        GameManager.instance.playerSpeedText.SetActive(true);

        // Marks the Speed Power-Up as activated in PlayerPrefs
        PlayerPrefs.SetInt("SpeedPowerUp", 1);

        // Handles speed doubling logic based on the current game mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            DoublePlayerSpeedForSingleplayer();
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                DoublePlayerSpeedForBothPlayersMultiplayerMode();
            }
            else
            {
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    DoublePlayerSpeedForPlayer1MultiplayerMode();
                }
                else if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    DoublePlayerSpeedForPlayer2MultiplayerMode();
                }
            }
        }

        // Destroys the Speed Power-Up object after applying its effect
        Destroy(gameObject);
    }

    private static void DoublePlayerSpeedForPlayer2MultiplayerMode()
    {
        // Doubles Player 2's speed in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
    }

    private static void DoublePlayerSpeedForPlayer1MultiplayerMode()
    {
        // Doubles Player 1's speed in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
    }

    private static void DoublePlayerSpeedForBothPlayersMultiplayerMode()
    {
        // Doubles speed for both players in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().playerSpeed *= 2.0f;
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerMultiplayer2>().playerSpeed *= 2.0f;
    }

    private static void DoublePlayerSpeedForSingleplayer()
    {
        // Doubles Player 1's speed in single-player mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().playerSpeed *= 2.0f;
    }

    private void ProcessShieldPowerUp()
    {
        // Handles shield activation logic based on the current game mode
        if (PlayerPrefs.GetInt("Controller", 0) == 0)
        {
            ActivateShieldsForSingleplayer();
        }
        else if (PlayerPrefs.GetInt("Controller", 0) == 1)
        {
            if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player.Count > 1)
            {
                ActivateShieldsForBothPlayersMultiplayerMode();
            }
            else
            {
                if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer1")
                {
                    ActivateShieldForPlayer1Multiplayer();
                }
                else if (SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].name == "PlayerMultiplayer2")
                {
                    ActivateShieldForPlayer2Multiplayer();
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

    private static void ActivateShieldForPlayer2Multiplayer()
    {
        // Activates shield for Player 2 in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = true;
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer2>().claireShield.SetActive(true);
    }

    private static void ActivateShieldForPlayer1Multiplayer()
    {
        // Activates shield for Player 1 in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = true;
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield.SetActive(true);
    }

    private static void ActivateShieldsForBothPlayersMultiplayerMode()
    {
        // Activates shields for both players in multiplayer mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = true;
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerHealth>().playerShield = true;

        // Activates shield visuals for both players
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerMultiplayer1>().remyShield.SetActive(true);
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[1].GetComponent<PlayerMultiplayer2>().claireShield.SetActive(true);
    }

    private static void ActivateShieldsForSingleplayer()
    {
        // Activates Player 1's shield in single-player mode
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerHealth>().playerShield = true;

        // Activates shield visuals for Player 1
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().remyShield.SetActive(true);
        SpawnManager.instance._levelSpawned.GetComponent<Levels>().player[0].GetComponent<PlayerController>().claireShield.SetActive(true);
    }
}
