using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // public variables
    public float playerSpeed = 6.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Animation playerAnimation; // Player's animation component
    public Transform remyBulletSpawnPosition; // Position where Remy's bullet is spawned
    public Transform claireBulletSpawnPosition; // Position where Claire's bullet is spawned
    public AudioClip fireSound; // Sound effect played when firing a bullet
    public GameObject remyShield, claireShield; // GameObjects representing shields for Remy and Claire

    // private variables
    private float _horizontalMovementKeyboard; // Horizontal movement input from keyboard
    private float _verticalMovementKeyboard; // Vertical movement input from keyboard
    private float _horizontalMovementXbox; // Horizontal movement input from Xbox controller
    private float _verticalMovementXbox; // Vertical movement input from Xbox controller
    private float _combinedHorizontal, _combinedVertical; // Combined horizontal and vertical axes for keyboard and Xbox controller
    private float _rightStickHorizontal, _rightStickVertical; // Used to get input for right stick of Xbox controller
    private Vector3 _playerMovement; // Player movement vector
    private readonly float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private readonly float _fireThreshold = 0.1f; // Threshold value for firing using controller (to prevent unintentional small movements)
    private readonly float _fireCooldown = 0.2f; // Cooldown time between firing bullets for controller input
    private float _lastFireTime; // Tracks the time of the last fired bullet using controller
    private float _nextFireTime = 0.1f; // Tracks the time when the player can fire the next bullet using mouse input
    private readonly float _fireRate = 0.2f; // Fire rate used for mouse input firing
    private int _avatar; // Stores the current avatar type (0 for Remy, 1 for Claire)
    private Vector3 _localMovement; // Local player movement direction based on player's transform
    private Vector3 _aimDirection; // Aim direction stores the direction player is facing
    private Vector3 _worldAimDirection; // Stores the converted vector from local space to world space
    
    private void Start()
    {
        // Load the selected avatar from PlayerPrefs, default to Remy (0)
        _avatar = PlayerPrefs.GetInt("Avatar", 0);
        if (_avatar == 0)
        {
            // Get the animation component for Remy
            playerAnimation = transform.GetChild(0).GetComponent<Animation>();
        }
        else if (_avatar == 1)
        {
            // Get the animation component for Claire
            playerAnimation = transform.GetChild(1).GetComponent<Animation>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Cutscene", 1) == 0)
        {
            // Gets player input for horizontal movement (left/right) from keyboard
            GetAxes();

            // Combines both keyboard and controller inputs for movement
            CombineAxes();

            MovePlayer();

            // Determines the aiming direction from the controller's right stick input
            _aimDirection = DetermineDirection(new Vector2(_rightStickHorizontal, _rightStickVertical));
            _worldAimDirection = transform.TransformDirection(_aimDirection);

            // Checks if player is moving and firing using mouse input
            if (IsPlayerMoving() && Input.GetMouseButton(0))
            {
                // Plays appropriate firing animation based on whether the speed power-up is active
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    PlayRunningFiringAnimation();
                }
                else
                {
                    PlayWalkingFiringAnimation();
                }

                // Fires a bullet if the cooldown period has elapsed
                if (Time.time >= _nextFireTime)
                {
                    FireBullet();
                    _nextFireTime = Time.time + _fireRate;
                }
            }
            else if (IsPlayerMoving() && _aimDirection != Vector3.zero) // Checks if the player is moving and aiming with the Xbox Controller
            {
                RotateUsingXboxControllerRightStick();

                // Plays appropriate firing animation based on whether the speed power-up is active
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    PlayRunningFiringAnimation();
                }
                else
                {
                    PlayWalkingFiringAnimation();
                }

                // Fires a bullet if aiming direction is provided and cooldown time has passed
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(_worldAimDirection);
                    _lastFireTime = Time.time;
                }
            }
            else if (!IsPlayerMoving() && _aimDirection != Vector3.zero) // Checks if player is not moving but aiming with controller input
            {
                RotateUsingXboxControllerRightStick();

                PlayShootingAnimation();

                // Fires a bullet if aiming direction is provided and cooldown time has passed
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(_worldAimDirection);
                    _lastFireTime = Time.time;
                }
            }
            else
            {
                StopShootingAnimation();

                // Checks if player is moving without firing
                if (IsPlayerMoving())
                {
                    // Plays appropriate walking or running animation based on power-up
                    if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                    {
                        PlayRunningAnimation();
                    }
                    else
                    {
                        PlayWalkingAnimation();
                    }
                }
                else
                {
                    // Play idle animation when player is not moving
                    PlayIdleAnimation();
                }

                // Handles firing animations for mouse input if the player is idle
                if (Input.GetMouseButton(0))
                {
                    PlayShootingAnimation();
                    if (Time.time >= _nextFireTime)
                    {
                        FireBullet();
                        _nextFireTime = Time.time + _fireRate;
                    }
                }
                else if (_aimDirection != Vector3.zero) // Handles firing animations for controller input if the player is idle
                {
                    PlayShootingAnimation();
                    if (Time.time >= _lastFireTime + _fireCooldown)
                    {
                        FireBulletXbox(_worldAimDirection);
                        _lastFireTime = Time.time;
                    }
                }
                else
                {
                    // Stops the firing animation if no input is detected
                    StopShootingAnimation();
                }
            }

            // Handles player rotation to face the aiming direction, either by mouse or Xbox controller input
            HandlePlayerRotation();
        }
        else
        {
            if (IsItTalkingCutsceneLevel())
            {
                PlayTalkingAnimation();
            }
            else
            {
                PlayIdleAnimation();
            }
        }
    }

    private void PlayTalkingAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyTalking");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireTalking");
        }
    }

    private static bool IsItTalkingCutsceneLevel()
    {
        return PlayerPrefs.GetInt("Level", 1) == 4 || PlayerPrefs.GetInt("Level", 1) == 5 ||
               PlayerPrefs.GetInt("Level", 1) == 6 || PlayerPrefs.GetInt("Level", 1) == 7;
    }

    private void PlayIdleAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyIdle");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireIdle");
        }
    }

    private void PlayWalkingAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyWalking");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireWalking");
        }
    }

    private void PlayRunningAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyRunning");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireRunning");
        }
    }

    private void RotateUsingXboxControllerRightStick()
    {
        _worldAimDirection.y = 0; // Keeps the aiming direction on the horizontal plane
        Quaternion targetRotation = Quaternion.LookRotation(_worldAimDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360);
    }

    private void PlayWalkingFiringAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyWalkingFiring");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireWalkingFiring");
        }
    }

    private void PlayRunningFiringAnimation()
    {
        if (_avatar == 0)
        {
            playerAnimation.Play("RemyRunningFiring");
        }
        else if (_avatar == 1)
        {
            playerAnimation.Play("ClaireRunningFiring");
        }
    }

    private bool IsPlayerMoving()
    {
        return (_combinedHorizontal != 0 || _combinedVertical != 0);
    }

    private void MovePlayer()
    {
        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_combinedHorizontal, 0, _combinedVertical).normalized;
        _localMovement = transform.TransformDirection(_playerMovement);
        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _localMovement * playerSpeed;
    }

    private void CombineAxes()
    {
        _combinedHorizontal = _horizontalMovementKeyboard + _horizontalMovementXbox;
        _combinedVertical = _verticalMovementKeyboard + _verticalMovementXbox;
    }

    private void GetAxes()
    {
        _horizontalMovementKeyboard = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (up/down) from keyboard
        _verticalMovementKeyboard = Input.GetAxis("Vertical");

        // Gets player input for horizontal movement (left/right) from Xbox controller
        _horizontalMovementXbox = Input.GetAxis("HorizontalPlayer2Xbox");
        // Gets player input for vertical movement (up/down) from Xbox controller
        _verticalMovementXbox = Input.GetAxis("VerticalPlayer2Xbox");

        // Gets input from the right stick for aiming direction (used in controllers)
        _rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        _rightStickVertical = Input.GetAxis("RightStickVertical");
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (_avatar == 0)
        {
            if (!playerAnimation.IsPlaying("RemyIdleFiring"))
            {
                playerAnimation.Play("RemyIdleFiring");
            }
        }
        else if (_avatar == 1)
        {
            if (!playerAnimation.IsPlaying("ClaireIdleFiring"))
            {
                playerAnimation.Play("ClaireIdleFiring");
            }
        }
    }

    // Stops the shooting animation if it is playing
    void StopShootingAnimation()
    {
        if (_avatar == 0)
        {
            if (playerAnimation.IsPlaying("RemyIdleFiring"))
            {
                playerAnimation.Stop("RemyIdleFiring");
            }
        }
        else if (_avatar == 1)
        {
            if (playerAnimation.IsPlaying("ClaireIdleFiring"))
            {
                playerAnimation.Stop("ClaireIdleFiring");
            }
        }
    }
    // Continue later
    // Determines the direction for aiming based on controller stick input
    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        if (_stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
            return Vector3.zero;

        _stickInput.Normalize();
        float angle = Mathf.Atan2(_stickInput.y, _stickInput.x) * Mathf.Rad2Deg;

        // Correcting direction determination to properly detect left direction
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right;
        else if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized;
        else if (angle >= 67.5f && angle < 112.5f) return Vector3.forward;
        else if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized;
        else if ((angle >= 157.5f && angle <= 180f) || (angle < -157.5f && angle >= -180f)) return Vector3.left;
        else if (angle >= -157.5f && angle < -112.5f) return new Vector3(-1, 0, -1).normalized;
        else if (angle >= -112.5f && angle < -67.5f) return Vector3.back;
        else if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized;

        return Vector3.zero;
    }

    // Fires a bullet in a specific direction based on controller input
    void FireBulletXbox(Vector3 _direction)
    {
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.4f);

        if (_avatar == 0)
        {
            // Instantiates the bullet prefab at Remy's spawn position
            GameObject bullet = Instantiate(bulletPrefab, remyBulletSpawnPosition.position,
                remyBulletSpawnPosition.rotation);

            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Sets the bullet's velocity in the specified direction
                bulletRb.velocity = _direction * _bulletSpeed;
            }

            // Destroys the bullet after 0.5 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 0.5f);
        }
        else if (_avatar == 1)
        {
            // Instantiates the bullet prefab at Claire's spawn position
            GameObject bullet = Instantiate(bulletPrefab, claireBulletSpawnPosition.position,
                claireBulletSpawnPosition.rotation);

            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Sets the bullet's velocity in the specified direction
                bulletRb.velocity = _direction * _bulletSpeed;
            }

            // Destroys the bullet after 0.5 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 0.5f);
        }
    }

    // Fires a bullet from the player's position using mouse input
    void FireBullet()
    {
        AudioSource.PlayClipAtPoint(fireSound, SpawnManager.instance.transform.position, 0.4f);

        if (_avatar == 0)
        {
            // Instantiates the bullet prefab at Remy's spawn position
            GameObject bullet = Instantiate(bulletPrefab, remyBulletSpawnPosition.position,
                remyBulletSpawnPosition.rotation);

            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = transform.forward * _bulletSpeed;
            }

            // Destroys the bullet after 0.5 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 0.5f);
        }
        else if (_avatar == 1)
        {
            // Instantiates the bullet prefab at Claire's spawn position
            GameObject bullet = Instantiate(bulletPrefab, claireBulletSpawnPosition.position,
                claireBulletSpawnPosition.rotation);
            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                // Creates a ray from the camera to the mouse position to determine where the bullet should go
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

            // Destroys the bullet after 0.5 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 0.5f);
        }
    }

    void HandlePlayerRotation()
    {
        float mouseDelta = Input.GetAxis("Mouse X");

        transform.Rotate(0, 200f * mouseDelta * Time.deltaTime, 0); // Adjust rotation speed if needed
    }
}