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

    private bool _dashQueued;
    private float _dashCharge;
    private bool _dashRechargeableDurationIsRunning;
    private bool _slowDashState;
    private float _dashEndTime;
    
    public event Action<Vector3> Dashed;
    
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

        _dashCharge = movement.MaxDashCharges;
    }
    void Update()
    {
        _moveInput = moveAction.action.ReadValue<Vector2>();
        
        if (jumpAction.action.IsPressed() && !coilback.IsRewinding)
        {
            _bufferTimer = movement.JumpBufferTime;
        }

        if (dashAction.action.WasPressedThisFrame() && !coilback.IsRewinding)
        {
            _dashQueued = true;
        }
    }

    void FixedUpdate()
    {
        // If Coilback is rewinding, nothing here can happen
        if (!coilback.IsRewinding)
        {
            // If the player is grounded dash recharges over time at the default rate (DashRechargeTime)
            if (_isGrounded)
            {
                // If dash rechargeable duration is running and the player grounds, the coroutine stops
                if (_dashRechargeableDurationIsRunning)
                {
                    StopCoroutine("DashRechargeableDuration");
                    
                    _dashRechargeableDurationIsRunning = false;
                    _slowDashState = false;
                }
                
                // Dash recharging over time (normal speed of DashRechargeTime)
                _dashCharge = Mathf.Min(_dashCharge + Time.fixedDeltaTime/movement.DashRechargeTime, movement.MaxDashCharges);
            }
            else
            {
                // If the player isn't grounded and dash rechargeable duration is not running, start the coroutine
                if (!_dashRechargeableDurationIsRunning)
                {
                    StartCoroutine("DashRechargeableDuration");
                }
                
                // If the player is in slowDashState (set by the coroutine) the dash recharges at DashRechargeTimeSlow
                else if (!_slowDashState)
                    _dashCharge = Mathf.Min(_dashCharge + Time.fixedDeltaTime/movement.DashRechargeTime, movement.MaxDashCharges);
                
                // While the coroutine is running and the player isn't in slow mode yet, recharge at the normal speed
                else
                    _dashCharge = Mathf.Min(_dashCharge + Time.fixedDeltaTime/movement.DashRechargeTimeSlow, movement.MaxDashCharges);
            }
        
            Vector3 wishDirection = GetWishDirection(_moveInput);
        
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
            CheckGround();

            // Jumping application
            bool jumped = TryJump();
            
            if (jumped)
                _dashEndTime = Time.time;
            
            // Dashing application
            bool dashed = TryConsumeDash();
            
            if (dashed)
                _dashEndTime = Time.time + movement.DashDuration;

            bool dashing = Time.time < _dashEndTime;

            bool skipHorizontal = dashed || (dashing && movement.DashLocksMovement);

            if (!skipHorizontal)
            {
                if (_isGrounded && !jumped)
                {
                    horizontalVelocity = ApplyFriction(horizontalVelocity);
                    horizontalVelocity = Accelerate(horizontalVelocity, wishDirection, movement.GroundWishSpeed, movement.GroundWishSpeed, movement.GroundAcceleration);
                }
                else
                {
                    float oldSpeed = horizontalVelocity.magnitude;
                    Vector3 newVelocity = Accelerate(horizontalVelocity, wishDirection, Mathf.Min(movement.AirWishSpeedCap, movement.GroundWishSpeed), movement.GroundWishSpeed, movement.AirAcceleration);
                    float newSpeed = newVelocity.magnitude;

                    if (newSpeed > oldSpeed)
                    {
                        float keptSpeed = oldSpeed + movement.AirSpeedGainScale * (newSpeed - oldSpeed);
                        newVelocity *= (keptSpeed / newSpeed);
                    }
                
                    horizontalVelocity = newVelocity;
                }
            }

            if (jumped)
            {
                _lastLeaveGroundTime = Time.time;
                float jumpVelocity = Mathf.Sqrt(2 * movement.Gravity * movement.JumpHeight);
                velocity.y = jumpVelocity;
            }
            else if (_isGrounded)
                velocity.y = -(horizontalVelocity.x * _groundNormal.x + horizontalVelocity.z * _groundNormal.z) / _groundNormal.y;
            else if (!dashing)
                velocity.y -= movement.Gravity * Time.fixedDeltaTime;
            
            if (dashed)
            {
                Vector3 full = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
                
                if (movement.DashCancelsFall && full.y < 0)
                    full.y = 0;

                Vector3 dashDirection = GetDashDirection(_moveInput);
                
                Dashed?.Invoke(dashDirection);
                
                full = ApplyDash(full, dashDirection);
                
                horizontalVelocity = new Vector3(full.x, 0, full.z);
                velocity.y = full.y;
                
                if (velocity.y > 0)
                {
                    _lastLeaveGroundTime = Time.time;
                }
            }   
            
            rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        }
    }
    
    private Vector3 ApplyDash(Vector3 full, Vector3 direction)
    {
        if (movement.DashRedirect)
        {
            float fullHorizontalSpeed = new Vector3(full.x, 0, full.z).magnitude;

            return (direction * (fullHorizontalSpeed + movement.DashSpeed));
        }

        return (full + direction * movement.DashSpeed);
    }
    
    // Coroutine for checking how long the player can be not grounded while recharging dash at the default rate (DashRechargeTime)
    private IEnumerator DashRechargeableDuration()
    {
        _dashRechargeableDurationIsRunning = true;
        yield return new WaitForSeconds(movement.DurationUntilSlowRecharge);
        
        _dashRechargeableDurationIsRunning = false;
        _slowDashState = true;
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
        
        if (_isGrounded == false && _moveInput.y == 0)
            return ((forward) + (right * moveInput.x)).normalized;
        
        // Unit length so diagonals are not faster (only input)
        return ((forward * moveInput.y) + (right * moveInput.x)).normalized;
    }

    private Vector3 GetDashDirection(Vector2 moveInput)
    {
        // Make 2 different forward, one flat and one not
        Vector3 forward = playerCamera.transform.forward;
        
        Vector3 forwardFlat = forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();
        
        Vector3 right = playerCamera.transform.right;
        Vector3 dashForward = forward;

        // Switch case for different movement modes
        switch (movement.DashDirectionMode)
        {
            
            case DashDirectionMode.FlatKeys:
                if (moveInput == Vector2.zero)
                    return forwardFlat;
                else
                    return GetWishDirection(moveInput);
                
            case DashDirectionMode.CameraLook:
                return forward;
                
            case DashDirectionMode.CameraRelativeKeys:
                
                if (moveInput == Vector2.zero)
                    return forward;
                else if (moveInput.y < 0 && movement.FlattenBackDash)
                    dashForward = forwardFlat;
                
                return (dashForward * moveInput.y + right * moveInput.x).normalized;
                
            default:
                return forward;
        }
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
       // check if player is grounded
            if (_bufferTimer > 0 && _isGrounded)
            {
                _bufferTimer = 0;
                return true;
            }
            _bufferTimer -= Time.fixedDeltaTime;
            return false;
    }

    private bool TryConsumeDash()
    {
        if (_dashQueued && _dashCharge >= 1)
        {
            _dashQueued = false;
            _dashCharge -= 1;
            return true;
        }
        _dashQueued = false;
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
    public float DashCharge
    {
        get { return _dashCharge; }
        set { _dashCharge = value; }
    }
    
    public float HorizontalSpeed => new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
    public float TotalSpeed => rb.linearVelocity.magnitude;
}
