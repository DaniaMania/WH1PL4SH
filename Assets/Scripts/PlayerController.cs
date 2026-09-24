using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInputDirection;
    private bool _jumpQueued;

    private bool _isGrounded = true;
    
    [Header("Movement Inputs")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    void Start()
    {
        // lock cursor to the middle of the screen (lock mode = locked caused cursor to not be visible as well)
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        _moveInputDirection = moveAction.action.ReadValue<Vector2>();
        
        if (jumpAction.action.WasPressedThisFrame())
        {
            _jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        PlayerJump();
    }

    private void PlayerJump()
    {
        // player jump is queued in Update
        if (_jumpQueued)
        {
            // check if player is grounded
            if (_isGrounded)
            {
                // insert jump method
            }
            _jumpQueued = false;
        }
    }
}
