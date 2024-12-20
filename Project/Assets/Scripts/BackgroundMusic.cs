using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic _instance; // Holds the singleton instance of BackgroundMusic

    // Ensures only one instance of BackgroundMusic exists across scenes
    private void Awake()
    {
        if (_instance == null) // If no instance exists, assigns this object as the instance
        {
            _instance = this; // Sets the instance to this object
            DontDestroyOnLoad(this); // Prevents this object from being destroyed when loading a new scene
        }
        else
        {
            if (this != _instance) // If another instance exists, destroys this object
            {
                Destroy(gameObject); // Destroys the duplicate BackgroundMusic object
            }
        }
    }
}