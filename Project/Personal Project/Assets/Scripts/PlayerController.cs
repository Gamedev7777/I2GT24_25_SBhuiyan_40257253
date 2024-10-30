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
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        _playerMovement = new Vector3(_horizontalMovement, 0, _verticalMovement).normalized;

        transform.Translate(_playerspeed * Time.deltaTime * _playerMovement, Space.World);
    }
}