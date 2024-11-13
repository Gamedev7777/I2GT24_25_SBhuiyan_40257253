using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplayer1 : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting

    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private float _fireThreshold = 0.1f; // Threshold value for firing
    private float _fireCooldown = 0.5f; // Cooldown time between firing bullets
    private float _lastFireTime; // Tracks the time of the last fired bullet
    private float _nextFireTime = 0.0f;
    private float _fireRate = 0.1f;
    private Animation animation;

    void Start()
    {
        animation = transform.GetChild(0).GetComponent<Animation>();
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
        
        

        
        if ((_horizontalMovement != 0 || _verticalMovement != 0) && Input.GetMouseButton(0))
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
        else
        {
            if (_horizontalMovement != 0 || _verticalMovement != 0)
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
            else
            {
                Debug.Log("Stop firing");
                StopShootingAnimation();
            }
        }
        
        
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
    
    
    // Function to fire a bullet from the player's position
    void FireBullet()
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

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