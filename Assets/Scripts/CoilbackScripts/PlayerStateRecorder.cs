using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRecorder : MonoBehaviour
{
    private Vector3 _playerPosition;
    private Vector3 _playerVelocity;
    private float _dashCharge;
    private List<(Vector3, Vector3, float, float)> _playerStates = new List<(Vector3, Vector3, float, float)>();
    
    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;

    private void FixedUpdate()
    {
        // Find player position, velocity, and dash charges every fixed update
        _playerPosition = playerController.transform.position;
        _playerVelocity = playerRb.linearVelocity;
        _dashCharge = playerController.DashCharge;
        
        // Add the relevant player state elements as a tuple to the list
        _playerStates.Add((_playerPosition, _playerVelocity, _dashCharge, Time.fixedDeltaTime));
        
        Debug.Log(_playerStates.Count.ToString());
    }
}
