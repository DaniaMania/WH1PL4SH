using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoilbackRewind : MonoBehaviour
{
    private bool _isRewinding;
    private int _newPlayerSequence;
    private float _coilbackCharge;
    
    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerStateRecorder playerStateRecorder;
    
    [Header("Rewind Input")]
    [SerializeField] private InputActionReference rewindAction;
    
    [Header("Coilback Settings")]
    [SerializeField] private CoilbackSettings coilback;

    private void Start()
    {
        _coilbackCharge = coilback.CoilBackMaxCharge;
    }

    private void FixedUpdate()
    {
        // Check if the player is rewinding and there is a previous sequence to play and the Coilback has charge
        if (_isRewinding && _newPlayerSequence > 1 && _coilbackCharge > 0)
        {
            // Set player to isKinematic so physics doesn't apply during rewind
            playerRb.isKinematic = true;
            
            // Rewinding player position
            playerRb.MovePosition(playerStateRecorder.PlayerPosition(_newPlayerSequence));
            playerStateRecorder.PlayerPosition(_newPlayerSequence--);
            
            // Rewinding dash charge
            playerController.DashCharge =  playerStateRecorder.DashBarAmount(_newPlayerSequence);

            // Reduce Coilback charge by decreaseAmount per FixedUpdate
            _coilbackCharge -= coilback.CoilbackDecreaseAmount;
        }
        else
        {
            CoilbackStopped();
            
            // Coilback charges on FixedUpdate while not using it
            if (_coilbackCharge < coilback.CoilBackMaxCharge)
                _coilbackCharge += Time.fixedDeltaTime / coilback.CoilbackRechargeTime;
        }
    }
    
    

    private void OnEnable()
    {
        // Rewind key pressed
        rewindAction.action.started += OnRewindStarted;
        
        // Rewind key released
        rewindAction.action.canceled += OnRewindReleased;
    }

    // Runs when rewind key is pressed
    private void OnRewindStarted(InputAction.CallbackContext context)
    {
        if (_coilbackCharge >= coilback.CoilBackMaxCharge * 0.25f)
        {
            _newPlayerSequence = playerStateRecorder.CurrentPlayerSequence();
            _isRewinding = true;
        }
    }

    // Runs when rewind key is released
    private void OnRewindReleased(InputAction.CallbackContext context)
    {
        CoilbackStopped();
    }

    // Method for anything that happens when Coilback is stopped
    private void CoilbackStopped()
    {
        _isRewinding = false;
        playerRb.isKinematic = false;
        // launching player code
        
    }

    // Public setters for PlayerController to read
    public bool IsRewinding => _isRewinding;
    
    public float CoilBackCharge => _coilbackCharge;
}
