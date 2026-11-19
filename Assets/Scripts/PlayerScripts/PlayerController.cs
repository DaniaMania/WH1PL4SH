using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VectorGraphics;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _dashRechargeable;
    private Vector3 _groundNormal;
    private float _lastLeaveGroundTime;
    private float _bufferTimer;
    
    [Header("Player References")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MovementSettings movement;
    [SerializeField] private CapsuleCollider playerCapsuleCollider;
    [SerializeField] private CoilbackRewind coilback;
    
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
        
        if (jumpAction.action.IsPressed() && !coilback.IsRewinding)
        {
            _bufferTimer = movement.JumpBufferTime;
        }
    }

    void FixedUpdate()
    {
        // If the Coilback rewind is happening, input actions in FixedUpdate don't happen
        if (coilback.IsRewinding) return;

        CheckGround();

        Vector3 wishDirection = GetWishDirection(_moveInput);
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 targetHorizontal = wishDirection * movement.WalkGroundSpeed;
        
        // Calculate horizontal and vertical velocity with the methods 
        Vector3 newHorizontal = CalculateHorizontalVelocity(wishDirection, currentHorizontal, targetHorizontal);
        float newVertical = CalculateVerticalVelocity(currentVelocity.y, currentHorizontal);

        // Apply velocities to player
        rb.linearVelocity = new Vector3(newHorizontal.x, newVertical, newHorizontal.z);
    }

    private Vector3 CalculateHorizontalVelocity(Vector3 wishDirection, Vector3 currentHorizontal, Vector3 targetHorizontal)
    {
        // rate float is the speed that the player's current position reaches the target position (horizontally)
        float rate;
        
        if (!_isGrounded)
            rate = movement.WalkAirAcceleration;
        else if (wishDirection == Vector3.zero)
            rate = movement.WalkFriction;
        else
            rate = movement.WalkGroundAcceleration;

        return Vector3.MoveTowards(currentHorizontal, targetHorizontal, rate * Time.fixedDeltaTime);
    }

    private float CalculateVerticalVelocity(float currentVertical, Vector3 currentHorizontal)
    {
        if (TryJump())
            // v^2 = u^2 + 2as, with u = 0 at the peak
            return Mathf.Sqrt(2f * movement.Gravity * movement.JumpHeight);

        if (_isGrounded)
            // Match the slope so the player sticks to the ground instead of launching off it
            return -(currentHorizontal.x * _groundNormal.x + currentHorizontal.z * _groundNormal.z) / _groundNormal.y;

        return currentVertical - movement.Gravity * Time.fixedDeltaTime;
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

    private bool TryJump()
    {
       // check if player is grounded
            if (_bufferTimer > 0 && _isGrounded)
            {
                _bufferTimer = 0;
                return true;
            }
            _bufferTimer -= Time.fixedDeltaTime;
            return false;
    }

    // Ground check based on sphere cast
    private void CheckGround()
    {
        if ((Time.time - _lastLeaveGroundTime) < movement.GroundIgnoreAfterJump)
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

    // Gizmos for sphere cast
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

    // Sphere ground cast for the player
    private (Vector3 origin, float radius, float distance) GetGroundCastGeometry()
    {
        Vector3 localBottomCenter = playerCapsuleCollider.center - Vector3.up * (playerCapsuleCollider.height / 2 - playerCapsuleCollider.radius);
        Vector3 bottomCenter = transform.TransformPoint(localBottomCenter);
        Vector3 origin = bottomCenter + Vector3.up * movement.GroundCheckStartOffset;
        float radius = playerCapsuleCollider.radius * movement.GroundCheckRadiusScale;
        float distance = movement.GroundCheckStartOffset + playerCapsuleCollider.radius * (1 - movement.GroundCheckRadiusScale) + movement.GroundCheckDistance;
        
        return (origin, radius, distance);
    } 
    
    // Reveal values so it can be used in DevTools
    public bool IsGrounded => _isGrounded;
    
    public float HorizontalSpeed => new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
    public float TotalSpeed => rb.linearVelocity.magnitude;
}
