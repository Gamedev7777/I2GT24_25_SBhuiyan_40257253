using UnityEditor.Build.Content;
using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    public static AlienHealth Instance;
    private bool _alienDeath = false;

    void Awake()
    {
        Instance = this;
    }

    private int _health = 10; // Set the initial health

    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0 && !_alienDeath)
        {
            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + 100);
            GameManager.instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score").ToString();
            Die(); // Call a method to handle the player's death
            Debug.Log("Die");
        }

        Debug.Log(_health);
    }

    void Die()
    {
        // Handle alien death
        _alienDeath = true;
        SpawnManager.Instance.alienList.Remove(gameObject);
        SpawnManager.Instance.LevelComplete();
        Debug.Log(gameObject.name);
        Destroy(gameObject);
    }
}