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
            GameManager.Instance.scoreText.text = "Score: " + PlayerPrefs.GetInt("Score").ToString();
            Die(); // Call a method to handle the alien's death
        }
        
    }

    void Die()
    {
        // Handle alien death
        _alienDeath = true;
        Debug.Log("Aliens are dead");
        SpawnManager.Instance.alienList.Remove(gameObject);
        SpawnManager.Instance.LevelComplete();
        Destroy(gameObject);
    }
}