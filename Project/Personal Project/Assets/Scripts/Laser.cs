using UnityEngine;
public class Laser : MonoBehaviour

{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth.instance.TakeDamage(1);
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the laser after 1 second
        Destroy(gameObject, 1.0f);
    }
}