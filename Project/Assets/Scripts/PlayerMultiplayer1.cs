using UnityEngine;

public class PlayerMultiplayer1 : MonoBehaviour
{
    // Public variables
    public float playerSpeed = 6.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition; // Position from where the bullet is fired
    public AudioClip fireSound; // Audio clip played when firing a bullet
    public GameObject remyShield; // Shield object

    // Private variables
    private float _horizontalMovement; // Stores horizontal movement input from player
    private float _verticalMovement; // Stores vertical movement input from player
    private Vector3 _playerMovement; // Vector representing player movement direction
    private readonly float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private float _nextFireTime = 0.1f; // Time when the player can next fire
    private readonly float _fireRate = 0.2f; // Rate at which bullets can be fired
    private Animation _remyAnimation; // Reference to the Animation component of the player model
    private Vector3 _localMovement; // Player's movement relative to their current rotation
    private GameObject _bullet; // Stores the instantiated bullet that the player fires
    
    private void Start()
    {
        // Gets the Animation component from the player's first child
        _remyAnimation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            GetAxes();

            MovePlayer();

            // Checks if player is moving and firing using mouse input
            if (IsPlayerMoving() && Input.GetMouseButton(0))
            {
                // Chooses the appropriate animation based on whether the player has the speed power-up
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    _remyAnimation.Play("RemyRunningFiring"); // Plays running and shooting animation
                }
                else
                {
                    _remyAnimation.Play("RemyWalkingFiring"); // Plays walking and shooting animation
                }

                FireBullet(); // Calls the function to fire a bullet  
            }
            else // Handles different player states when not both moving and firing
            {
                StopShootingAnimation();
                
                // If player is moving but not firing, plays movement animations
                if (IsPlayerMoving())
                {
                    // Chooses the appropriate animation based on whether the player has the speed power-up
                    if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                    {
                        _remyAnimation.Play("RemyRunning"); // Plays running animation
                    }
                    else
                    {
                        _remyAnimation.Play("RemyWalking"); // Plays walking animation
                    }
                }
                else // If the player is not moving, plays the idle animation
                {
                    _remyAnimation.Play("RemyIdle"); // Plays the idle animation
                }

                // If the player is not moving but holding the left mouse button, starts firing
                if (Input.GetMouseButton(0))
                {
                    PlayShootingAnimation(); // Plays idle shooting animation

                    FireBullet(); // Calls the function to fire a bullet
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
            if (IsItTalkingCutsceneLevel())
            {
                _remyAnimation.Play("RemyTalking"); // Plays the Remy talking animation
            }
            else
            {
                _remyAnimation.Play("RemyIdle"); // Plays the Remy idle animation
            }
        }
    }

    private static bool IsItTalkingCutsceneLevel()
    {
        return PlayerPrefs.GetInt("Level", 1) == 4 || PlayerPrefs.GetInt("Level", 1) == 5 ||
               PlayerPrefs.GetInt("Level", 1) == 6 || PlayerPrefs.GetInt("Level", 1) == 7; // Checking if the level is 4,5,6 or 7 as talking cutscenes play before them
    }

    private bool IsPlayerMoving()
    {
        return (_horizontalMovement != 0 || _verticalMovement != 0); // Checks whether the player is moving
    }

    private void MovePlayer()
    {
        // Creates a new movement vector based on the player's input, normalizing for diagonal movement
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;
        
        // Converts the movement to be relative to the player's current rotation
        _localMovement = transform.TransformDirection(_playerMovement);

        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _localMovement * playerSpeed;
    }

    private void GetAxes()
    {
        _horizontalMovement = Input.GetAxis("Horizontal"); // Gets horizontal axis

        _verticalMovement = Input.GetAxis("Vertical"); // Gets vertical axis
    }

    // Method to play the shooting animation when player starts firing while idle
    private void PlayShootingAnimation()
    {
        // Plays the shooting animation only if it's not already playing
        if (!_remyAnimation.IsPlaying("RemyIdleFiring"))
        {
            _remyAnimation.Play("RemyIdleFiring");
        }
    }

    // Method to stop the shooting animation when player stops firing
    private void StopShootingAnimation()
    {
        // Stops the shooting animation if it is currently playing
        if (_remyAnimation.IsPlaying("RemyIdleFiring"))
        {
            _remyAnimation.Stop("RemyIdleFiring");
        }
    }

    // Method to fire a bullet from the player's position
    private void FireBullet()
    {
        if (Time.time >= _nextFireTime)
        {
            PlayFiringSound();

            InstantiateBullet();

            LaunchBullet();
            
            _nextFireTime = Time.time + _fireRate; // Updates the next time the player can fire
        }
    }

    private void LaunchBullet()
    {
        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = _bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = transform.forward * _bulletSpeed; // Sets the velocity of the bullet
        }
        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(_bullet, 1.0f);
    }

    private void InstantiateBullet()
    {
        // Instantiates the bullet prefab at the player's current position
        _bullet = Instantiate(bulletPrefab, bulletSpawnPosition.position, bulletSpawnPosition.rotation);
    }

    private void PlayFiringSound()
    {
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.4f); // Plays firing sound
    }


    private void HandlePlayerRotation()
    {
        float mouseDelta = Input.GetAxis("Mouse X");

        transform.Rotate(0, 200f * mouseDelta * Time.deltaTime, 0); // Adjusts rotation speed if needed
    }
}