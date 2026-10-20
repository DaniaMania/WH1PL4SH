using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class CoilbackRewind : MonoBehaviour
{
    private bool _isRewinding;
    private int _newPlayerSequence;
    private float _coilbackCharge;
    
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
                _coilbackCharge += Time.fixedDeltaTime / (coilback.CoilbackRechargeTime / 100);
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
            
            // Fades in the rewinding overlay (temporary)
            rewindOverlay.DOFade(0.1f, 1.0f);
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
        
        // Fades out the rewinding overlay if the Coilback is stopped for any reason (temporary)
        rewindOverlay.DOFade(0, 1.0f);
        
        // TODO launching player code
        
    }

    // Public setters for PlayerController to read
    public bool IsRewinding => _isRewinding;
    
    public float CoilBackCharge => _coilbackCharge;
}
