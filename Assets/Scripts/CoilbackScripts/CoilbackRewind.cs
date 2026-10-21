using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CoilbackRewind : MonoBehaviour
{
    private bool _isRewinding;
    private bool _rewindOnCooldown;
    private int _newPlayerSequence;
    private float _coilbackCharge;
    private int _rewindedSequenceCount;
    
    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerStateRecorder playerStateRecorder;
    
    [Header("UI References")]
    [SerializeField] private Image rewindOverlay;
    
    [Header("Rewind Input")]
    [SerializeField] private InputActionReference rewindAction;
    
    [Header("Coilback Settings")]
    [SerializeField] private CoilbackSettings coilback;

    private void Awake()
    {
        // Set Coilback charge to max on awake
        _coilbackCharge = coilback.CoilBackMaxCharge;
        
        // Checking the rewind action key if it is pressed or released
        rewindAction.action.performed += OnRewind;
        rewindAction.action.canceled += OnRewind;
    }
    
    // OnEnable/Disable so that Unity doesn't check for keyboard inputs when Coilback isn't relevant
    private void OnEnable()
    {
        rewindAction.action.Enable();
    }

    private void OnDisable()
    {
        rewindAction.action.Disable();
    }

    private void FixedUpdate()
    {
        // Check if the player is rewinding and there is a previous sequence to play and the Coilback has charge and the Coilback is not on cooldown
        if (_isRewinding && _newPlayerSequence > 1 && _coilbackCharge > 0 && !_rewindOnCooldown)
        {
            // Set player to isKinematic so physics doesn't apply during rewind
            playerRb.isKinematic = true;

            // Rewinding player position
            playerRb.MovePosition(playerStateRecorder.PlayerPosition(_newPlayerSequence));
            
            // If there are enough sequences that has happened so far sequence count divided by 10 
            if (_newPlayerSequence > _rewindedSequenceCount/10)
                playerStateRecorder.PlayerPosition(_newPlayerSequence -= _rewindedSequenceCount/10);
            // 
            else
            {
                playerStateRecorder.PlayerPosition(_newPlayerSequence -= (_newPlayerSequence - 1));
                CoilbackStopped();
            }
            
            // Increment how many sequences the player has rewound in one instance
            _rewindedSequenceCount++;
            
            // Rewinding dash charge
            playerController.DashCharge =  playerStateRecorder.DashBarAmount(_newPlayerSequence);

            // Reduce Coilback charge by decreaseAmount per FixedUpdate
            _coilbackCharge -= coilback.CoilbackDecreaseAmount;
        }
        // Coilback is stopped automatically if there is no charge
        else if (_coilbackCharge < 0)
        {
            CoilbackStopped();
        }
        else
        {
            // Coilback charges on FixedUpdate while not using it
            if (_coilbackCharge < coilback.CoilBackMaxCharge)
                _coilbackCharge += Time.fixedDeltaTime / (coilback.CoilbackRechargeTime / 100);
        }
    }

    // Rewind key pressed
    private void OnRewind(InputAction.CallbackContext context)
    {
        // Runs when key is held down
        if (context.performed && !_rewindOnCooldown)
        {
            if (_coilbackCharge >= coilback.CoilBackMaxCharge * 0.25f)
            {
                _newPlayerSequence = playerStateRecorder.CurrentPlayerSequence();
                _isRewinding = true;
            
                // Fades in the rewinding overlay (temporary)
                rewindOverlay.DOFade(0.1f, 1.0f);
            }
        }
        // Runs when key is released
        else if (context.canceled && _isRewinding)
        {
            CoilbackStopped();
        }
    }

    // Method for anything that happens when Coilback is stopped
    private void CoilbackStopped()
    {
        _isRewinding = false;
        playerRb.isKinematic = false;
        _rewindedSequenceCount = 0;
        
        // Fades out the rewinding overlay if the Coilback is stopped for any reason (temporary)
        rewindOverlay.DOFade(0, 1.0f);

        StartCoroutine(CoilbackCooldown());
        
    }
    
    // Coroutine for Coilback Cooldown
    private IEnumerator CoilbackCooldown()
    {
        _rewindOnCooldown = true;
        yield return new WaitForSeconds(coilback.CoilbackCooldownDuration);
        _rewindOnCooldown = false;
    }

    // Public setters for PlayerController to read
    public bool IsRewinding => _isRewinding;
    
    public bool RewindOnCooldown => _rewindOnCooldown;
    
    public float CoilBackCharge => _coilbackCharge;
}
