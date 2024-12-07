using UnityEngine;

public class PlayerMultiplayer1 : MonoBehaviour
{
    // Public variables
    public float playerSpeed = 6.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition; // Position from where the bullet is fired
    public LayerMask groundLayer; // Layer mask for ground to detect player rotation
    public AudioClip fireSound; // Audio clip played when firing a bullet
    public GameObject remyShield; // Shield object

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
    private Vector3 _localMovement; // Player's movement relative to their current rotation
    private Vector3 lastMousePosition;

    void Start()
    {
        // Gets the Animation component from the player's child GameObject
        animation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            // Gets player input for horizontal movement (A/D or Left/Right keys)
            _horizontalMovement = Input.GetAxis("Horizontal");
            // Gets player input for vertical movement (W/S or Up/Down keys)
            _verticalMovement = Input.GetAxis("Vertical");

            // Creates a new movement vector based on the player's input, normalizing for diagonal movement
            _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;
            // Converts the movement to be relative to the player's current rotation
            _localMovement = transform.TransformDirection(_playerMovement);

            // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
            transform.GetComponent<Rigidbody>().velocity = _localMovement * playerSpeed;

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
                    _nextFireTime = Time.time + _fireRate; // Updates the next time the player can fire
                }
            }
            else // Handles different player states when not both moving and firing
            {
                StopShootingAnimation();
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
                        _nextFireTime = Time.time + _fireRate; // Updates the next time the player can fire
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
        else
        {
            if (PlayerPrefs.GetInt("Level", 1) == 4 || PlayerPrefs.GetInt("Level", 1) == 5 ||
                PlayerPrefs.GetInt("Level", 1) == 6 || PlayerPrefs.GetInt("Level", 1) == 7)
            {
                animation.Play("RemyTalking");
            }
            else
            {
                animation.Play("RemyIdle");
            }
        }
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
        // Stops the shooting animation if it is currently playing
        if (animation.IsPlaying("RemyIdleFiring"))
        {
            animation.Stop("RemyIdleFiring");
        }
    }

    // Method to fire a bullet from the player's position
    void FireBullet()
    {
        // Plays the fire sound effect at the position of the SpawnManager
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.4f);

        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = transform.forward * _bulletSpeed;
        }

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
    }


    void HandlePlayerRotation()
    {
        float mouseDelta = Input.GetAxis("Mouse X");

        transform.Rotate(0, 200f * mouseDelta * Time.deltaTime, 0); // Adjust rotation speed if needed
    }
}