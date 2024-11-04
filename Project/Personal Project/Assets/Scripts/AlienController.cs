using UnityEngine;

public class AlienController : MonoBehaviour
{
    // public variable
    // Laser prefab to be used for shooting
    public GameObject laserPrefab;

    // private variables
    private float _alienSpeed = 10.0f; // Alien movement speed
    private float _rotationSpeed = 10.0f; // Alien rotation speed
    private Vector3 _direction; // Direction for the alien to move towards
    private Quaternion _lookRotation; // Rotation for the alien to look towards the player
    private float _distanceToPlayer; // Distance between the alien and the player
    private readonly float _stopDistance = 5.0f; // Minimum distance to stop moving towards the player
    private float _laserSpeed = 30.0f; // Speed of the laser shot by the alien
    private float _fireTimer; // Timer to track firing intervals
    private float _fireInterval = 2.0f; // Time interval between firing lasers
    private Rigidbody _laserRb; // Rigidbody component of the laser
    private Vector3 _laserDirection; // Direction in which the laser will be fired

    // Start is called before the first frame update
    void Start()
    {
        // Initialises the fire timer with the firing interval
        _fireTimer = _fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the player is still alive before performing any actions
        if (!PlayerHealth.Instance.playerDeath)
        {
            // Calculates the distance between the alien and the player
            _distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

            // If the distance to the player is greater than the stop distance, move towards the player
            if (_distanceToPlayer > _stopDistance)
            {
                // Moves the alien towards the player's position
                transform.position = Vector3.MoveTowards(transform.position,
                    PlayerController.Instance.transform.position, _alienSpeed * Time.deltaTime);
                // Calculates the direction towards the player
                _direction = (PlayerController.Instance.transform.position - transform.position).normalized;
                // Sets the rotation to look towards the player
                _lookRotation = Quaternion.LookRotation(_direction);
                // Smoothly rotates the alien to face the player
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, _lookRotation, _rotationSpeed * Time.deltaTime);
            }

            // Decreases the fire timer by the time elapsed since the last frame
            _fireTimer -= Time.deltaTime;
            // If the fire timer reaches zero, fire a laser
            if (_fireTimer <= 0)
            {
                FireLaser();
                // Resets the fire timer after firing a laser
                _fireTimer = _fireInterval;
            }
        }
    }

    // Function to handle firing of the laser
    void FireLaser()
    {
        // Checks if the laser prefab is assigned
        if (laserPrefab != null)
        {
            // Calculates the direction from the alien to the player
            _laserDirection = (PlayerController.Instance.transform.position - transform.position).normalized;
            // Sets the rotation of the laser to face the direction it will move
            Quaternion _laserRotation = Quaternion.LookRotation(_laserDirection);
            // Instantiates the laser at the alien's position with the calculated rotation
            GameObject laser = Instantiate(laserPrefab, transform.position, _laserRotation);
            // Gets the Rigidbody component of the laser prefab
            _laserRb = laser.GetComponent<Rigidbody>();
            // Sets the velocity of the laser in the calculated direction
            _laserRb.velocity = _laserDirection * _laserSpeed;
        }
    }
}