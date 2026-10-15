using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRecorder : MonoBehaviour
{
    private Vector3 _playerPosition;
    private Vector3 _playerVelocity;
    private float _dashCharge;
    private List<(Vector3 playerPosition, Vector3 playerVelocity, float dashCharge, float time)> _playerStates = new List<(Vector3 playerPosition, Vector3 playerVelocity, float dashCharge, float time)>();
    private int _currentPlayerSequence;
    
    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;

    private void FixedUpdate()
    {
        RecordPosition();
    }

    /* Records player position, velocity, dash charge amount, and current time in a tuple in a list */
    private void RecordPosition()
    {
        // Find player position, velocity, and dash charge amount
        _playerPosition = playerController.transform.position;
        _playerVelocity = playerRb.linearVelocity;
        _dashCharge = playerController.DashCharge;
        
        // Add the relevant player state elements as a tuple to the list
        _playerStates.Add((_playerPosition, _playerVelocity, _dashCharge, Time.fixedTime));
        
        // Debug that prints out all of the tuple values
        //Debug.Log(" Position: " +  _playerStates[_currentPlayerSequence].playerPosition + " Time: " + _playerStates[_currentPlayerSequence].time + " Velocity: " + _playerStates[_currentPlayerSequence].playerVelocity.magnitude + " Time: " + _playerStates[_currentPlayerSequence].time);
        
        // Increment the position sequence by 1
        _currentPlayerSequence++;
    }

    public int CurrentPlayerSequence()
    {
        return _currentPlayerSequence;
    }
    
    public Vector3 PlayerPosition(int currentPlayerSequence)
    {
        return _playerStates[currentPlayerSequence - 1].playerPosition;
    }

    public float PlayerVelocity()
    {
        return _playerStates[_currentPlayerSequence - 1].playerVelocity.magnitude;
    }

    public float DashBarAmount()
    {
        return _playerStates[_currentPlayerSequence - 1].dashCharge;
    }

    public float CurrentTimeElapsed()
    {
        return _playerStates[_currentPlayerSequence - 1].time;
    }
}
