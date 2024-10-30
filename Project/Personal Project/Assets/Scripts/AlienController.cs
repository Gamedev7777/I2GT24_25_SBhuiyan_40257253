using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    private float _alienSpeed = 3.0f;
    private float _rotationSpeed = 3.0f;

    public Transform targetPlayer;
    private Vector3 _direction;
    private Quaternion _lookRotation;
    private float _distanceToPlayer;
    private readonly float _stopDistance = 5.0f;
    public GameObject laserPrefab;
    private float _laserSpeed = 10.0f;
    private float _fireTimer;
    private float _fireInterval = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        _fireTimer = _fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
    // Calculating distance between player and alien and saving it in the _distanceToPlayer variable
        _distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        
        // Checking if the alien is far enough from the player
        
        if (_distanceToPlayer > _stopDistance)
        {
        //Handles the movement of the alien following the player and the rotation of the alien to face the player
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, _alienSpeed * Time.deltaTime);
            _direction = (targetPlayer.position - transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, _rotationSpeed * Time.deltaTime);
        }
        _fireTimer -= Time.deltaTime;
    }
}