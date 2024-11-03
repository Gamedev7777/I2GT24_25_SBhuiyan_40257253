using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    
    {
        if (gameObject.tag == "PowerUpHealth")
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerHealth.Instance.health += 20;
                GameManager.Instance.playerHealthText.text = "Player Health: " + PlayerHealth.Instance.health.ToString();
                Destroy(gameObject);
            }   
        }

        if (gameObject.tag == "PowerUpSpeed")
        {
            if (other.gameObject.tag == "Player")
            {
                GameManager.Instance.playerSpeedText.SetActive(true);
                PlayerController.Instance.playerSpeed *= 2.0f;
                Destroy(gameObject);
            }
        }

        if (gameObject.tag == "PowerUpShield")
        {
            if (other.gameObject.tag == "Player")
            {
               GameManager.Instance.playerShieldText.SetActive(true);
                PlayerHealth.Instance.playerShield = true;
               SpawnManager.Instance.ProcessPlayerShield();
               Destroy(gameObject);
            }
        }
    }

    
    
}
