using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] private InputActionReference dashAction;

    void Start()
    {
        // lock cursor to the middle of the screen (lock mode = locked caused cursor to not be visible as well)
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        _moveInput = moveAction.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // If Coilback is rewinding, nothing here can happen
        if (!coilback.IsRewinding)
        {
            CheckGround();
            
            Vector3 wishDirection = GetWishDirection(_moveInput);
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

            Vector3 targetVelocity = wishDirection * movement.WalkGroundSpeed;

            Vector3 nextHorizontal;
            
            if (_isGrounded)
            {
                if (wishDirection == new Vector3(0,0,0))
                    nextHorizontal= Vector3.MoveTowards(horizontalVelocity, targetVelocity, movement.WalkFriction * Time.fixedDeltaTime);
                else
                    nextHorizontal= Vector3.MoveTowards(horizontalVelocity, targetVelocity, movement.WalkGroundAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                nextHorizontal= Vector3.MoveTowards(horizontalVelocity, targetVelocity, movement.WalkAirAcceleration * Time.fixedDeltaTime);
            }
            //horizontalVelocity = new Vector3(wishDirection.x * 10, 0, wishDirection.z * 10);
            
            rb.linearVelocity = new Vector3(nextHorizontal.x, velocity.y, nextHorizontal.z);
        }
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
    
    // Reveal values so it can be used in DevTools
    public bool IsGrounded => _isGrounded;
    
    public float HorizontalSpeed => new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
    public float TotalSpeed => rb.linearVelocity.magnitude;
}
