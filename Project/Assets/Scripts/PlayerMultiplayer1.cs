using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer1 : MonoBehaviour
{
    // Public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition;
    // Private variables
    private float _horizontalMovement; // Stores horizontal movement input from player
    private float _verticalMovement; // Stores vertical movement input from player
    private Vector3 _playerMovement; // Vector representing player movement direction
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Minimum time interval allowed between firing bullets
    private float _fireCooldown = 0.5f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private float _nextFireTime = 0.1f; // Time when the player can next fire
    private float _fireRate = 0.2f; // Rate at which bullets can be fired
    private Animation animation; // Reference to the Animation component of the player model
    public AudioClip fireSound;
    public GameObject remyShield;
    
    void Start()
    {
        // Gets the Animation component from the player's child GameObject
        animation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (A/D or Left/Right keys)
        _horizontalMovement = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (W/S or Up/Down keys)
        _verticalMovement = Input.GetAxis("Vertical");

        // Creates a new movement vector based on the player's input, normalizing for diagonal movement
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _playerMovement * playerSpeed;

        // If player is moving and holding the left mouse button, play the firing movement animation and fire bullet
        if ((_horizontalMovement != 0 || _verticalMovement != 0) && Input.GetMouseButton(0))
        {
            // Chooses the appropriate animation based on whether the player has the speed power-up
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                animation.Play("RemyRunningFiring"); // Plays running and shooting animation
            }
            else
            {
                animation.Play("RemyWalkingFiring"); // Plays walking and shooting animation
            }

            // Fires a bullet if the cooldown has elapsed
            if (Time.time >= _nextFireTime)
            {
                FireBullet(); // Calls the function to fire a bullet  
                _nextFireTime = Time.time + _fireRate;
            }
        }
        else // Handles different player states when not both moving and firing
        {
            // If player is moving but not firing, plays movement animations
            if (_horizontalMovement != 0 || _verticalMovement != 0)
            {
                // Chooses the appropriate animation based on whether the player has the speed power-up
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    animation.Play("RemyRunning"); // Plays running animation
                }
                else
                {
                    animation.Play("RemyWalking"); // Plays walking animation
                }
            }
            else // If the player is not moving, plays the idle animation
            {
                animation.Play("RemyIdle"); // Plays the idle animation
            }

            // If the player is not moving but holding the left mouse button, starts firing
            if (Input.GetMouseButton(0))
            {
                PlayShootingAnimation(); // Plays idle shooting animation
                if (Time.time >= _nextFireTime)
                {
                    FireBullet(); // Calls the function to fire a bullet
                    _nextFireTime = Time.time + _fireRate;
                }
            }
            else
            {
                StopShootingAnimation(); // Stops the idle shooting animation
            }
        }

        // Handles the rotation of the player based on the mouse position
        HandlePlayerRotation();
    }

    // Method to play the shooting animation when player starts firing while idle
    void PlayShootingAnimation()
    {
        // Plays the shooting animation only if it's not already playing
        if (!animation.IsPlaying("RemyIdleFiring"))
        {
            animation.Play("RemyIdleFiring");
        }
    }

    // Method to stop the shooting animation when player stops firing
    void StopShootingAnimation()
    {
        if (animation.IsPlaying("RemyIdleFiring"))
        {
           animation.Stop("RemyIdleFiring");
        }
    }

    // Method to fire a bullet from the player's position
    void FireBullet()
    {
        AudioSource.PlayClipAtPoint(fireSound, Camera.main.transform.position, 0.4f);
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            // Creates a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Performs a raycast to determine where the mouse cursor intersects with the game world
            if (Physics.Raycast(ray, out hit))
            {
                // Calculates the direction from the player's position to the target hit point
                Vector3 direction = (hit.point - transform.position).normalized;
                // Sets the bullet's velocity in the direction of the target
                bulletRb.velocity = direction * _bulletSpeed;
            }
        }

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
    }

    // Method to handle player rotation to face the mouse cursor
    void HandlePlayerRotation()
    {
        // Creates a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Performs a raycast to determine where the mouse cursor intersects with the game world
        if (Physics.Raycast(ray, out hit))
        {
            // Calculates the direction from the player's position to the target hit point
            Vector3 direction = (hit.point - transform.position).normalized;
            direction.y = 0; // Keeps the player's rotation on the horizontal plane to avoid tilting
            Quaternion newRotation = Quaternion.LookRotation(direction); // Creates a new rotation to face the target
            transform.rotation = newRotation; // Applies the new rotation to the player
        }
    }
}
