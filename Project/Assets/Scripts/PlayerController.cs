using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Animation animation; // Player's animation component

    public Transform remyBulletSpawnPosition;

    public Transform claireBulletSpawnPosition;
    // private variables
    private float _horizontalMovementKeyboard; // Horizontal movement input from keyboard
    private float _verticalMovementKeyboard; // Vertical movement input from keyboard
    private float _horizontalMovementXbox; // Horizontal movement input from Xbox controller
    private float _verticalMovementXbox; // Vertical movement input from Xbox controller
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves

    private float
        _fireThreshold = 0.1f; // Threshold value for firing using controller (to prevent unintentional small movements)

    private float _fireCooldown = 0.2f; // Cooldown time between firing bullets for controller input
    private float _lastFireTime; // Tracks the time of the last fired bullet using controller
    private float _nextFireTime = 0.1f; // Tracks the time when the player can fire the next bullet using mouse input
    private float _fireRate = 0.2f; // Fire rate used for mouse input firing
    private int _avatar;
    public AudioClip fireSound;
    public GameObject remyShield, claireShield;

    void Start()
    {
        _avatar = PlayerPrefs.GetInt("Avatar", 0);
        if (_avatar == 0)
        {
            animation = transform.GetChild(0).GetComponent<Animation>();
        }
        else if (_avatar == 1)
        {
            animation = transform.GetChild(1).GetComponent<Animation>();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (left/right) from keyboard
        _horizontalMovementKeyboard = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (up/down) from keyboard
        _verticalMovementKeyboard = Input.GetAxis("Vertical");

        // Gets player input for horizontal movement (left/right) from Xbox controller
        _horizontalMovementXbox = Input.GetAxis("HorizontalPlayer2Xbox");
        // Gets player input for vertical movement (up/down) from Xbox controller
        _verticalMovementXbox = Input.GetAxis("VerticalPlayer2Xbox");

        // Combines both keyboard and controller inputs for movement
        float _combinedHorizontal = _horizontalMovementKeyboard + _horizontalMovementXbox;
        float _combinedVertical = _verticalMovementKeyboard + _verticalMovementXbox;

        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_combinedHorizontal, 0, _combinedVertical).normalized;

        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _playerMovement * playerSpeed;

        // Gets input from the right stick for aiming direction (used in controllers)
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");

        // Determines the aiming direction from the controller's right stick input
        Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));

        // Checks if player is moving and firing using mouse input
        if ((_combinedHorizontal != 0 || _combinedVertical != 0) && Input.GetMouseButton(0))
        {
            // Plays appropriate firing animation based on whether the speed power-up is active
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                if (_avatar == 0)
                {
                    animation.Play("RemyRunningFiring");
                }
                else if (_avatar == 1)
                {
                    animation.Play("ClaireRunningFiring");
                }
                
            }
            else
            {
                if (_avatar == 0)
                {
                    animation.Play("RemyWalkingFiring");
                }
                else if (_avatar == 1)
                {
                    animation.Play("ClaireWalkingFiring");
                }
            }

            // Fires a bullet if the cooldown period has elapsed
            if (Time.time >= _nextFireTime)
            {
                FireBullet();
                _nextFireTime = Time.time + _fireRate;
            }
        }
        // Checks if player is not moving but aiming with controller input
        else if ((_combinedHorizontal == 0 && _combinedVertical == 0) && (aimDirection != Vector3.zero))
        {
            // Plays appropriate firing animation based on whether the speed power-up is active
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                if (_avatar == 0)
                {
                    animation.Play("RemyRunningFiring");
                }
                else if (_avatar == 1)
                {
                    animation.Play("ClaireRunningFiring");
                }
            }
            else
            {
                if (_avatar == 0)
                {
                    animation.Play("RemyWalkingFiring");
                }
                else if (_avatar == 1)
                {
                    animation.Play("ClaireWalkingFiring");
                }
                
            }

            // Fires a bullet if aiming direction is provided and cooldown time has passed
            if (Time.time >= _lastFireTime + _fireCooldown)
            {
                FireBulletXbox(aimDirection);
                _lastFireTime = Time.time;
            }
        }
        else
        {
            // Checks if player is moving without firing
            if (_combinedHorizontal != 0 || _combinedVertical != 0)
            {
                // Plays appropriate walking or running animation based on power-up
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    if (_avatar == 0)
                    {
                        animation.Play("RemyRunning");
                    }
                    else if (_avatar == 1)
                    {
                        animation.Play("ClaireRunning");
                    }
                }
                else
                {
                    if (_avatar == 0)
                    {
                        animation.Play("RemyWalking");
                    }
                    else if (_avatar == 1)
                    {
                        animation.Play("ClaireWalking");
                    }
                }
            }
            else
            {
                if (_avatar == 0)
                {
                    animation.Play("RemyIdle");
                }
                else if (_avatar == 1)
                {
                    animation.Play("ClaireIdle");
                }
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
            // Handles firing animations for controller input if the player is idle
            else if (aimDirection != Vector3.zero)
            {
                PlayShootingAnimation();
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(aimDirection);
                    _lastFireTime = Time.time;
                }
            }
            else
            {
                // Stops the firing animation if no input is detected
                StopShootingAnimation();
            }
        }

        // Handles player rotation to face the aiming direction, either by mouse or controller input
        HandlePlayerRotation();
    }

    // Plays the shooting animation if it is not already playing
    void PlayShootingAnimation()
    {
        if (_avatar == 0)
        {
            if (!animation.IsPlaying("RemyIdleFiring"))
            {
                animation.Play("RemyIdleFiring");
            }
        }
        else if (_avatar == 1)
        {
            if (!animation.IsPlaying("ClaireIdleFiring"))
            {
                animation.Play("ClaireIdleFiring");
            }
        }
        
    }

    // Stops the shooting animation if it is playing
    void StopShootingAnimation()
    {
        if (_avatar == 0)
        {
            if (animation.IsPlaying("RemyIdleFiring"))
            {
                animation.Stop("RemyIdleFiring");
            }   
        }
        else if (_avatar == 1)
        {
            if (animation.IsPlaying("ClaireIdleFiring"))
            {
                animation.Stop("ClaireIdleFiring");
            }
        }
        
    }

    // Determines the direction based on input from the right stick
    Vector3 DetermineDirection(Vector2 _stickInput)
    {
        // If the input magnitude is below the threshold, returns zero vector (no direction)
        if (_stickInput.sqrMagnitude < _fireThreshold * _fireThreshold)
        {
            return Vector3.zero; // No firing direction if stick input is minimal
        }

        // Normalises the stick input to get a consistent direction
        _stickInput.Normalize();

        // Converts the input angle to a direction vector for bullet firing
        float angle = Mathf.Atan2(_stickInput.y, _stickInput.x) * Mathf.Rad2Deg;
        if (angle >= -22.5f && angle < 22.5f) return Vector3.right; // Right direction
        else if (angle >= 22.5f && angle < 67.5f) return new Vector3(1, 0, 1).normalized; // Diagonal right-forward
        else if (angle >= 67.5f && angle < 112.5f) return Vector3.forward; // Forward direction
        else if (angle >= 112.5f && angle < 157.5f) return new Vector3(-1, 0, 1).normalized; // Diagonal left-forward
        else if ((angle >= 157.5f && angle < 180f) || (angle < -157f && angle >= -180f))
            return Vector3.left; // Left direction
        else if (angle >= -157.5f && angle < -112.5f)
            return new Vector3(-1, 0, -1).normalized; // Diagonal left-backward
        else if (angle >= -112.5f && angle < -67.5f) return Vector3.back; // Backward direction
        else if (angle >= -67.5f && angle < -22.5f) return new Vector3(1, 0, -1).normalized; // Diagonal right-backward
        return Vector3.zero; // Default case (no direction)
    }

    // Fires a bullet in a specific direction based on controller input
    void FireBulletXbox(Vector3 _direction)
    {
        AudioSource.PlayClipAtPoint(fireSound, Camera.main.transform.position, 0.4f);
        
        if (_avatar == 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, remyBulletSpawnPosition.position, Quaternion.identity);

            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Sets the bullet's velocity in the specified direction
                bulletRb.velocity = _direction * _bulletSpeed;
            }

            // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 1.0f);   
        }
        else if (_avatar == 1)
        {
            GameObject bullet = Instantiate(bulletPrefab, claireBulletSpawnPosition.position, Quaternion.identity);

            // Gets the Rigidbody component of the bullet to control its movement
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Sets the bullet's velocity in the specified direction
                bulletRb.velocity = _direction * _bulletSpeed;
            }

            // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
            Destroy(bullet, 1.0f);
        }
        
        // Instantiates the bullet prefab at the player's current position
        
    }

    // Fires a bullet from the player's position using mouse input
    void FireBullet()
    {
        AudioSource.PlayClipAtPoint(fireSound, Camera.main.transform.position, 0.4f);
        
        
        if (_avatar == 0)
        {
            // Instantiates the bullet prefab at the player's current position
            GameObject bullet = Instantiate(bulletPrefab, remyBulletSpawnPosition.position, Quaternion.identity);
         
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
            Destroy(bullet, 1.0f);

        }
        else if (_avatar == 1)
        {
            GameObject bullet = Instantiate(bulletPrefab, claireBulletSpawnPosition.position, Quaternion.identity);
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
            Destroy(bullet, 1.0f);
            
        }
        
        
        
        

        

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        
    }

    // Handles player rotation based on the mouse cursor position
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