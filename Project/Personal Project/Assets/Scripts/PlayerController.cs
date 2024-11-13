using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting


    // private variables
    private float _horizontalMovementKeyboard; // Horizontal movement input
    private float _verticalMovementKeyboard; // Vertical movement input
    private float _horizontalMovementXbox; // Horizontal movement input
    private float _verticalMovementXbox; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing using controller
    private float _fireCooldown = 0.1f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private float _nextFireTime = 0.0f;
    private float _fireRate = 0.1f;

    public Animation animation;

    void Start()
    {
        animation = transform.GetChild(0).GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (left/right)
        _horizontalMovementKeyboard = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (up/down)
        _verticalMovementKeyboard = Input.GetAxis("Vertical");


        // Gets player input for horizontal movement (left/right)
        _horizontalMovementXbox = Input.GetAxis("HorizontalPlayer2Xbox");
        // Gets player input for vertical movement (up/down)
        _verticalMovementXbox = Input.GetAxis("VerticalPlayer2Xbox");


        float _combinedHorizontal = _horizontalMovementKeyboard + _horizontalMovementXbox;
        float _combinedVertical = _verticalMovementKeyboard + _verticalMovementXbox;

        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_combinedHorizontal, 0, _combinedVertical).normalized;

        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _playerMovement * playerSpeed;


        // Gets input from the right stick for aiming direction (used in controllers)
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");

        Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));

        

        if ((_combinedHorizontal != 0 || _combinedVertical != 0) && Input.GetMouseButton(0))
        {
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                Debug.Log("Running and firing");
                animation.Play("RemyRunningFiring");
            }
            else
            {
                Debug.Log("Walking and firing");

                animation.Play("RemyWalkingFiring");
            }

            if (Time.time >= _nextFireTime)
            {
                FireBullet(); // Calls the function to fire a bullet  
                _nextFireTime = Time.time + _fireRate;
            }
        }
        else if ((_combinedHorizontal == 0 && _combinedVertical == 0) && (aimDirection != Vector3.zero))
        {
            if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
            {
                Debug.Log("Running and firing");
                animation.Play("RemyRunningFiring");
            }
            else
            {
                Debug.Log("Walking and firing");

                animation.Play("RemyWalkingFiring");
            }
            
            // Fires a bullet if aiming direction is provided and cooldown time has passed
            if (Time.time >= _lastFireTime + _fireCooldown)
            {
                FireBulletXbox(aimDirection);
                _lastFireTime = Time.time; // Updates the last fire time
            }
        }
        else
        {
            if (_combinedHorizontal != 0 || _combinedVertical != 0)
            {
                if (PlayerPrefs.GetInt("SpeedPowerUp", 0) == 1)
                {
                    Debug.Log("Running");
                    animation.Play("RemyRunning");
                }
                else
                {
                    Debug.Log("Walking");
                    animation.Play("RemyWalking");    
                }
                
            }
            else
            {
                Debug.Log("Idle");
                animation.Play("RemyIdle");
            }
            
            
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Start firing");
                PlayShootingAnimation();
                if (Time.time >= _nextFireTime)
                {
                    FireBullet(); // Calls the function to fire a bullet  
                    _nextFireTime = Time.time + _fireRate;
                }
            }
            else if (aimDirection != Vector3.zero)
            {
                Debug.Log("Start firing");
                PlayShootingAnimation();
                if (Time.time >= _lastFireTime + _fireCooldown)
                {
                    FireBulletXbox(aimDirection);
                    _lastFireTime = Time.time; // Updates the last fire time
                }
                
            }
            else
            {
                Debug.Log("Stop firing");
                StopShootingAnimation();
            }
        }
        
        
        
        
        

        

        // Handles player rotation to face the aiming direction, either by mouse or controller input
        HandlePlayerRotation();
    }

    void PlayShootingAnimation()
    {
        // Play the shooting animation only if it's not already playing


        if (!animation.IsPlaying("RemyIdleFiring"))
        {
            Debug.Log("Shooting");
            animation.Play("RemyIdleFiring");
        }
    }

    void StopShootingAnimation()
    {
        if (animation.IsPlaying("RemyIdleFiring"))
        {
            Debug.Log("Not Shooting");
            animation.Stop("RemyIdleFiring");
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

        // Normalizes the stick input to get a consistent direction
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
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bullet fired"); // Debug log to indicate a bullet was fired

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

// Function to fire a bullet from the player's position using mouse input
    void FireBullet()
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bullet fired"); // Debug log to indicate a bullet was fired

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

        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
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