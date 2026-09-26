using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput;
    private bool _jumpQueued;
    private bool _isGrounded = true;
    
    [Header("Player References")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MovementSettings movement;
    
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
        _moveInput = moveAction.action.ReadValue<Vector2>();
        
        if (jumpAction.action.WasPressedThisFrame())
        {
            _jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 wishDirection = GetWishDirection(_moveInput);
        
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        bool jumped = TryJump();

        if (_isGrounded && !jumped)
        {
            horizontalVelocity = ApplyFriction(horizontalVelocity);
            horizontalVelocity = Accelerate(horizontalVelocity, wishDirection, movement.GroundWishSpeed, movement.GroundAcceleration);
        }
        else
        {
            // nothing for now
        }
        
        rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
    }

    /* Changes 2D input into 3D world direction relative to camera*/
    private Vector3 GetWishDirection(Vector2 moveInput)
    {
        // ignore camera pitch then normalize so looking down or up doesn't slow movement
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        right.Normalize();
        
        // Unit length so diagonals are not faster (only input)
        return ((forward * moveInput.y) + (right * moveInput.x)).normalized;
    }

    private Vector3 ApplyFriction(Vector3 horizontalVelocity)
    {
        float speed = horizontalVelocity.magnitude;
        
        if (speed <= 0.01f)
            return Vector3.zero;
        
        // Below stop speed, friction acts as if moving at stop speed (faster stop)
        float control = Mathf.Max(speed, movement.StopSpeed);
        float drop = control * movement.Friction * Time.fixedDeltaTime;
        float newSpeed = Mathf.Max(speed - drop, 0);
        
        return horizontalVelocity * (newSpeed / speed);
    }

    /* Pushes velocity toward wishDirection until velocity along wishDirection reaches wishspeed (however, general speed is uncapped)*/
    private Vector3 Accelerate(Vector3 horizontalVelocity, Vector3 wishDirection, float wishSpeed, float acceleration)
    {
        float currentSpeed = Vector3.Dot(horizontalVelocity, wishDirection);
        float addSpeed = wishSpeed - currentSpeed;

        // No acceleration if it is already at wishSpeed
        if (addSpeed <= 0)
            return horizontalVelocity;
        
        float accelerationSpeed = acceleration * wishSpeed * Time.fixedDeltaTime;
        accelerationSpeed = Mathf.Min(accelerationSpeed, addSpeed);
        
        return horizontalVelocity + wishDirection * accelerationSpeed;
    }

    private bool TryJump()
    {
        // player jump is queued in Update
        if (_jumpQueued)
        {
            _jumpQueued = false;
            
            // check if player is grounded
            if (_isGrounded)
            {
                // insert jump method
                return true;
            }
            return false;
        }
        return false;
    }
}
