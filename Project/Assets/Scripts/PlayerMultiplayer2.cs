using UnityEngine;

public class PlayerMultiplayer2 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 6.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Transform bulletSpawnPosition; // Position from where the bullet will spawn
    public GameObject claireShield; // Shield for the player
    public AudioClip fireSound; // Sound effect played when firing

    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private float _rightStickHorizontal; // Used to get input for right stick of Xbox controller 
    private float _rightStickVertical; // Used to get input for right stick of Xbox controller
    private Vector3 _playerMovement; // Player movement vector
    private readonly float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private readonly float _fireThreshold = 0.1f; // Threshold value for firing
    private readonly float _fireCooldown = 0.2f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private Animation _claireAnimation; // Player animation component
    private Vector3 _localMovement; // Player movement in local space
    private Vector3 _aimDirection; // Aim direction stores the direction player is facing
    private Vector3 _worldAimDirection; // Stores the converted vector from local space to world space
    private GameObject _bullet; // Stores the instantiated bullet that the player fires
    
    private void Start()
    {
        // Gets the Animation component attached to the player's child object
        _claireAnimation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            GetAxes();

            MovePlayer();

            _aimDirection = DetermineDirection(new Vector2(_rightStickHorizontal, _rightStickVertical)); // Calls the function that calculates the direction vector and stores it
            _worldAimDirection = transform.TransformDirection(_aimDirection); // Converts direction vector to world space using TransformDirection function

            // Determines if the player is both moving and aiming, and handles animation and firing
            if (IsPlayerMoving() && IsAimed())
            {
                RotateWithRightStick();

                // Checks if the speed power-up is active and plays appropriate animation
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    _claireAnimation.Play("ClaireRunningFiring"); // Plays running while firing animation
                }
                else
                {
                    _claireAnimation.Play("ClaireWalkingFiring"); // Plays walking while firing animation
                }
                
                FireBulletXbox(_worldAimDirection);
            }
            else
            {
                // Handles movement when player is not aiming
                if (IsPlayerMoving())
                {
                    // Checks if the speed power-up is active and plays appropriate animation
                    if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                    {
                        _claireAnimation.Play("ClaireRunning"); // Plays running animation
                    }
                    else
                    {
                        _claireAnimation.Play("ClaireWalking"); // Plays walking animation
                    }
                }
                else
                {
                    // Player is idle
                    _claireAnimation.Play("ClaireIdle"); // Plays idle animation
                }

                // Handles firing when the player is stationary or just aiming
                if (IsAimed())
                {
                    RotateWithRightStick();
                    // Plays shooting animation if aiming
                    PlayShootingAnimation();
                    
                    FireBulletXbox(_worldAimDirection);
                }
                else
                {
                    // Stops shooting animation if not aiming
                    StopShootingAnimation();
                }
            }
        }
        else
        {
            _claireAnimation.Play("ClaireIdle"); // Plays idle animation
        }
    }

    private void RotateWithRightStick()
    {
        // Keeps the aim direction aligned on the horizontal plane
        _worldAimDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(_worldAimDirection);
        // Rotates the player towards the aim direction
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 180);
    }

    private bool IsAimed()
    {
        return (_aimDirection != Vector3.zero); // Checks if the right stick has tilted or not. If it has tilted then it aims in that direction
    }

    private bool IsPlayerMoving()
    {
        return (_horizontalMovement != 0 && _verticalMovement != 0); // Checks if the player is moving
    }

    private void MovePlayer()
    {
        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;
        _localMovement = transform.TransformDirection(_playerMovement);
        // Moves the player based on the input and playerSpeed
        transform.GetComponent<Rigidbody>().velocity = _localMovement * playerSpeed;
    }

    private void GetAxes()
    {
        // Gets player input for horizontal movement (left/right)
        _horizontalMovement = Input.GetAxis("HorizontalPlayer2Xbox");
        // Gets player input for vertical movement (up/down)
        _verticalMovement = Input.GetAxis("VerticalPlayer2Xbox");
        // Gets input from the right stick for aiming direction
        _rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        _rightStickVertical = Input.GetAxis("RightStickVertical");
    }

    // Plays the shooting animation if it is not already playing
    private void PlayShootingAnimation()
    {
        if (!_claireAnimation.IsPlaying("ClaireIdleFiring"))
        {
            _claireAnimation.Play("ClaireIdleFiring"); // Plays idle firing animation
        }
    }

    // Stops the shooting animation if it is currently playing
    private void StopShootingAnimation()
    {
        if (_claireAnimation.IsPlaying("ClaireIdleFiring"))
        {
            _claireAnimation.Stop("ClaireIdleFiring"); // Stops idle firing animation
        }
    }

    // Determines the direction based on the input from the right stick
    private Vector3 DetermineDirection(Vector2 stickInput)
    {
        // Checks if the stick input magnitude is below the threshold to ignore minor movements
        if (stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
            return Vector3.zero;

        stickInput.Normalize();
        float angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;

        // Correcting direction determination to properly detect left direction
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right;
        if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized;
        if (angle >= 67.5f && angle < 112.5f) return Vector3.forward;
        if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized;
        if ((angle >= 157.5f && angle <= 180f) || (angle < -157.5f && angle >= -180f)) return Vector3.left;
        if (angle >= -157.5f && angle < -112.5f) return new Vector3(-1, 0, -1).normalized;
        if (angle >= -112.5f && angle < -67.5f) return Vector3.back;
        if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized;

        return Vector3.zero;
    }

    // Fires a bullet in a specific direction based on controller input
    private void FireBulletXbox(Vector3 direction)
    {
        // Fires a bullet if aiming direction is provided and cooldown time has passed
        if (Time.time >= _lastFireTime + _fireCooldown)
        {
            PlayFiringSound();

            InstantiateBullet();

            LaunchBullet(direction);

            _lastFireTime = Time.time; // Updates the last fire time
        }
    }

    private void LaunchBullet(Vector3 direction)
    {
        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = _bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            // Sets the bullet's velocity in the specified direction
            bulletRb.velocity = direction * _bulletSpeed;
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
        // Plays the fire sound effect at the SpawnManager's position
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.2f);
    }
}