using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour

{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        PlayerHealth.instance.TakeDamage(1);
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
