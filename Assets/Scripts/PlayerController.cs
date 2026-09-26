using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput;
    private bool _jumpQueued;
    private bool _isGrounded;
    private Vector3 _groundNormal;
    private float _lastJumpTime;
    
    [Header("Player References")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MovementSettings movement;
    [SerializeField] private CapsuleCollider playerCapsuleCollider;
    
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
        
        CheckGround();

        // Jumping and gravity application
        bool jumped = TryJump();
        
        if (_isGrounded && !jumped)
        {
            horizontalVelocity = ApplyFriction(horizontalVelocity);
            horizontalVelocity = Accelerate(horizontalVelocity, wishDirection, movement.GroundWishSpeed, movement.GroundWishSpeed,movement.GroundAcceleration);
        }
        else
        {
            horizontalVelocity = Accelerate(horizontalVelocity, wishDirection, Mathf.Min(movement.AirWishSpeedCap, movement.GroundWishSpeed), movement.GroundWishSpeed, movement.AirAcceleration);
        }

        if (jumped)
        {
            _lastJumpTime = Time.time;
            float jumpVelocity = Mathf.Sqrt(2 * movement.Gravity * movement.JumpHeight);
            velocity.y = jumpVelocity;
        }
        else if (_isGrounded)
            velocity.y = -(horizontalVelocity.x * _groundNormal.x + horizontalVelocity.z * _groundNormal.z) / _groundNormal.y;
        else
            velocity.y -= movement.Gravity * Time.fixedDeltaTime;
        
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

    /* Pushes velocity toward wishDirection until velocity along wishDirection reaches capSpeed (however, general speed is uncapped)*/
    private Vector3 Accelerate(Vector3 horizontalVelocity, Vector3 wishDirection, float capSpeed, float pushSpeed, float acceleration)
    {
        float currentSpeed = Vector3.Dot(horizontalVelocity, wishDirection);
        float addSpeed = capSpeed - currentSpeed;

        // No acceleration if it is already at capSpeed
        if (addSpeed <= 0)
            return horizontalVelocity;
        
        float accelerationSpeed = Mathf.Min(acceleration * pushSpeed * Time.fixedDeltaTime, addSpeed);
        
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
                return true;
            }
            return false;
        }
        return false;
    }

    private void CheckGround()
    {
        if ((Time.time - _lastJumpTime) < movement.GroundIgnoreAfterJump)
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            return;
        }

        var (origin, radius, distance) = GetGroundCastGeometry();

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, distance, movement.GroundLayers,
                QueryTriggerInteraction.Ignore) && Vector3.Angle(hit.normal, Vector3.up) <= movement.MaxSlopeAngle)
        {
            _isGrounded = true;
            _groundNormal = hit.normal;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
        }
    }

    private void OnDrawGizmos()
    {
        if (movement == null || playerCapsuleCollider == null)
            return;
        
        var (origin, radius, distance) = GetGroundCastGeometry();
        if (Application.isPlaying == false)
            Gizmos.color = Color.grey;
        else if (_isGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(origin, radius);
        Gizmos.DrawWireSphere(origin + Vector3.down * distance, radius);
        Gizmos.DrawLine(origin, origin + Vector3.down * distance);
    }

    private (Vector3 origin, float radius, float distance) GetGroundCastGeometry()
    {
        Vector3 localBottomCenter = playerCapsuleCollider.center - Vector3.up * (playerCapsuleCollider.height / 2 - playerCapsuleCollider.radius);
        Vector3 bottomCenter = transform.TransformPoint(localBottomCenter);
        Vector3 origin = bottomCenter + Vector3.up * movement.GroundCheckStartOffset;
        float radius = playerCapsuleCollider.radius * movement.GroundCheckRadiusScale;
        float distance = movement.GroundCheckStartOffset + playerCapsuleCollider.radius * (1 - movement.GroundCheckRadiusScale) + movement.GroundCheckDistance;
        
        return (origin, radius, distance);
    } 
}
