using UnityEngine;

public class TestNewInput : MonoBehaviour
{
    // Reference to the NewControls class, which will manage the player's input
    public NewControls xboxController;
    
    // Stores the player's movement input as a 2D vector (x and y direction)
    private Vector2 _movement;

    // Start is called before the first frame update
    // This method is used to initialize the script and set up necessary variables
    void Start()
    {
        // Instantiate the NewControls object to handle input actions
        xboxController = new NewControls();
    }

    // Update is called once per frame
    // This method constantly checks for changes in input and performs necessary actions
    void Update()
    {
        // Read the movement input value from the Player's movement action
        // Movement is represented as a Vector2, capturing both horizontal and vertical input
        _movement = xboxController.Player.Movement.ReadValue<Vector2>();
        
        // Print the current movement values to the console for debugging
        print(_movement);
    }
}