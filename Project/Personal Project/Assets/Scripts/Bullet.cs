using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void
        OnTriggerEnter(
            Collider other) // Detects the collision of two game objects if the on trigger boolean property is enabled
    {
        if (other.gameObject.tag == "Alien")
        {
            other.gameObject.GetComponent<AlienHealth>().TakeDamage(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the laser after 1 second
        Destroy(gameObject, 1.0f);
    }
}