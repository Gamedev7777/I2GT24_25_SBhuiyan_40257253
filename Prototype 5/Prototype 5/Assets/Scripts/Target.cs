using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody _targetRb;
    private float _minSpeed = 12f;
    private float _maxSpeed = 16f;
    private float _maxTorque = 10f;
    private float xRange = 4f;
    private float ySpawnPos = -6f;

    // Start is called before the first frame update
    void Start()
    {
        _targetRb = GetComponent<Rigidbody>();

        // Apply random upward force and random torque
        _targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        _targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        
        // Set the initial spawn position
        transform.position = RandomSpawnPos();
    }

    // Generates a random upward force
    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(_minSpeed, _maxSpeed);
    }

    // Generates a random torque for rotation
    float RandomTorque()
    {
        return Random.Range(-_maxTorque, _maxTorque);
    }

    // Generates a random spawn position within the xRange
    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, 0);
    }
}