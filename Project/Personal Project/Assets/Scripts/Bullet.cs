using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) // Detects the collision of two game objects if the on trigger boolean property is enabled
    {
        AlienHealth.Instance.TakeDamage(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the laser after 1 second
        Destroy(gameObject, 1.0f);
    }
}

