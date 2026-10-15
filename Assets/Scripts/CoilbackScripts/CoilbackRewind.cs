using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoilbackRewind : MonoBehaviour
{
    private bool _isRewinding;
    private int _newPlayerSequence;
    
    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerStateRecorder playerStateRecorder;
    
    [Header("Rewind Input")]
    [SerializeField] private InputActionReference rewindAction;

    private void Update()
    {
        if (_isRewinding)
        {
            // player state debug log
            Debug.Log("Current Position: " + playerStateRecorder.PlayerPosition(playerStateRecorder.CurrentPlayerSequence()));
            //Debug.Log("Current Velocity: " + playerStateRecorder.PlayerVelocity());
            //Debug.Log("Current Dash Bar Amount: " + playerStateRecorder.DashBarAmount());
            //Debug.Log("Current Time: " + playerStateRecorder.CurrentTimeElapsed());
            
            // rewinding code
            //Debug.Log("Rewind held");
            
            // rewind player position
            if (_newPlayerSequence > 1)
            {
                playerController.transform.position = playerStateRecorder.PlayerPosition(_newPlayerSequence);
                _newPlayerSequence--;
            }
        }
    }

    private void OnEnable()
    {
        // Rewind pressed
        rewindAction.action.started += OnRewindStarted;
        
        // Rewind released
        rewindAction.action.canceled += OnRewindReleased;
    }

    private void OnRewindStarted(InputAction.CallbackContext context)
    {
        _newPlayerSequence = playerStateRecorder.CurrentPlayerSequence();
        
        _isRewinding = true;
        Debug.Log("Rewind pressed");
    }

    private void OnRewindReleased(InputAction.CallbackContext context)
    {
        _isRewinding = false;
        
        // launching player code
        Debug.Log("Rewind released");
    }
}
