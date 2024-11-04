using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNewInput : MonoBehaviour
{
    public NewControls xboxController;
    private Vector2 _movement;
    // Start is called before the first frame update
    void Start()
    {
        xboxController = new NewControls();
    }

    
    // Update is called once per frame
    void Update()
    {
        _movement = xboxController.Player.Movement.ReadValue<Vector2>();
        print(_movement);
    }
}
