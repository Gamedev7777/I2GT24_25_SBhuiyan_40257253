using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float _playerspeed = 5.0f;
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _playerMovement;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMovement = 0;
        _verticalMovement = 0;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            _verticalMovement = 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            _verticalMovement = -1f; 
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            _horizontalMovement = -1f;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            _horizontalMovement = 1f;
        }
        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        transform.Translate(_playerspeed * Time.deltaTime * _playerMovement, Space.World);
    }
}