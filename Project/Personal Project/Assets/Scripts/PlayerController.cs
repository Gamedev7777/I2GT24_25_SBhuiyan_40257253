using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    public Animator animator;

    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing using controller
    private float _fireCooldown = 0.5f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private float _timeSinceStopped;
    private float _stopBufferTime;

    void Start()
    {
        _timeSinceStopped = 0.0f;
        _stopBufferTime = 0.2f;
        //animator = transform.GetChild(0).GetComponent<Animator>();
        Debug.Log(transform.GetChild(0).name);
        transform.GetChild(0).GetComponent<Animation>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        // Gets player input for horizontal movement (left/right)
        _horizontalMovement = Input.GetAxis("Horizontal");
        // Gets player input for vertical movement (up/down)
        _verticalMovement = Input.GetAxis("Vertical");


        // Creates a new movement vector based on the player's input
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        // Moves the player based on the input and playerSpeed, using Time.deltaTime for frame rate independence
        transform.GetComponent<Rigidbody>().velocity = _playerMovement * playerSpeed;

        Debug.Log("Velocity: " + transform.GetComponent<Rigidbody>().velocity.magnitude);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            _timeSinceStopped = 0.0f;
            Debug.Log("Walking");
            // animator.SetFloat("PlayerSpeed", smoothSpeed);
            //animator.SetFloat("PlayerSpeed", 1.0f);
        }
        else
        {
            _timeSinceStopped += Time.deltaTime;
            Debug.Log("Idle");
            if (_timeSinceStopped > _stopBufferTime)
            {
                // animator.SetFloat("PlayerSpeed", 0.0f);
                //animator.SetFloat("PlayerSpeed", Mathf.Lerp(animator.GetFloat("PlayerSpeed"), 0.0f, 0.1f));
            }
        }


        // Gets input from the right stick for aiming direction (used in controllers)
        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");
        Vector3 aimDirection = DetermineDirection(new Vector2(rightStickHorizontal, rightStickVertical));

        // Fires a bullet if aiming direction is provided and cooldown time has passed
        if (aimDirection != Vector3.zero && Time.time >= _lastFireTime + _fireCooldown)
        {
            FireBulletXbox(aimDirection);
            _lastFireTime = Time.time; // Updates the last fire time
        }

        // Checks if the left mouse button is pressed to fire a bullet
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet(); // Calls the function to fire a bullet
        }

        // Handles player rotation to face the aiming direction, either by mouse or controller input
        HandlePlayerRotation();
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