using UnityEngine;
using UnityEngine.InputSystem;

public class XboxPlayerController : MonoBehaviour
{
    // public variables
    public float playerSpeed = 5.0f; // Speed at which the player moves
    public GameObject bulletPrefab; // Bullet prefab used for shooting
    
    // private variables
    private float _horizontalMovement; // Horizontal movement input
    private float _verticalMovement; // Vertical movement input
    private Vector3 _playerMovement; // Player movement vector
    private float _bulletSpeed = 30.0f; // Speed at which the bullet moves
    private NewControls _xboxController; // Input system reference for Xbox controller
    private Vector2 _fireDirection; // Stores the direction of fire based on right stick input
    private bool _canFire = false; // Tracks if the player can fire a bullet

    void Start()
    {
        // Initialize input system controls
        _xboxController = new NewControls();
        // Bind fire action to handling function
        _xboxController.Player.Fire.performed += HandleXboxControllerFireInput;
        //_xboxController.Player.Fire.canceled += HandleXboxControllerFireInput;
        // Enable input controls
        _xboxController.Enable();
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

        // Moves the player based on the input and playerSpeed
        transform.Translate(playerSpeed * Time.deltaTime * _playerMovement, Space.World);

        // Checks if the fire button is pressed to fire a bullet
        if (_canFire)
        {
            FireBullet(); // Calls the function to fire a bullet
        }
    }

    // Function to fire a bullet from the player's position
    void FireBullet()
    {
        // Instantiates the bullet prefab at the player's current position
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bullet fired"); // Debug log to indicate a bullet was fired

        // Gets the Rigidbody component of the bullet to control its movement
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        
        // Get the direction from the right stick input
        _fireDirection = _xboxController.Player.FireDirection.ReadValue<Vector2>();
        Vector3 bulletDirection = new Vector3(_fireDirection.x, 0, _fireDirection.y).normalized;
        
        // Set the bullet's velocity to move in the specified direction
        bulletRb.velocity = bulletDirection * _bulletSpeed;
        
        // Reset firing flag to prevent continuous firing
        _canFire = false;
        
        // Destroys the bullet after 1 second to avoid cluttering the scene with unused bullets
        Destroy(bullet, 1.0f);
    }

    // Handles the input action for firing based on the context of the input
    void HandleXboxControllerFireInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _canFire = true; // Set to true when the fire button is pressed
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _canFire = false; // Set to false when the fire button is released
        }
    }
}
