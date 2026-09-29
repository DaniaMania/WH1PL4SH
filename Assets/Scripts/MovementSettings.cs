using UnityEngine;
public enum DashDirectionMode {FlatKeys, CameraLook, CameraRelativeKeys}

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Header("Ground Movement")] 
    [Min(0), Tooltip("Speed your input aims for on the ground. Actual speed can exceed this with other movement methods. Higher = faster running on the ground.")]
    [SerializeField] private float groundWishSpeed = 8;
    public float GroundWishSpeed => groundWishSpeed; 
    
    [Min(0), Tooltip("How quickly you reach ground wish speed. Higher = faster build up.")]
    [SerializeField] private float groundAcceleration = 10;
    public float GroundAcceleration => groundAcceleration;
    
    [Min(0), Tooltip("How fast ground speed decreases. Higher = less slide.")]
    [SerializeField] private float friction = 4;
    public float Friction => friction;
    
    [Min(0), Tooltip("Below this speed, friction acts as if you were moving this fast, so you stop cleanly instead of creeping.")]
    [SerializeField] private float stopSpeed = 2.5f;
    public float StopSpeed => stopSpeed;
    
    [Min(0), Tooltip("Amount of time that the player doesn't experience friction for after landing on the ground.")]
    [SerializeField] private float landingGraceTime = 0.1f;
    public float LandingGraceTime => landingGraceTime;
    
    
    
    [Header("Air Movement")] 
    [Range(0,10), Tooltip("Changes how forgiving the strafe angle is. Doesn't limit air speed.")]
    [SerializeField] private float airWishSpeedCap = 0.75f;
    public float AirWishSpeedCap => airWishSpeedCap;
    
    [Range(0, 10), Tooltip("Limits air braking and reversing.")]
    [SerializeField] private float airAcceleration = 4;
    public float AirAcceleration => airAcceleration;
    
    [Range(0, 1), Tooltip("How much speed from air strafing is kept. (1 = max, 0 = none)")]
    [SerializeField] private float airSpeedGainScale = 1;
    public float AirSpeedGainScale => airSpeedGainScale;
    
    
    
    [Header("Jump and Gravity")]
    [Min(0), Tooltip("How high you can jump. Jump is calculated from this and gravity.")]
    [SerializeField] private float jumpHeight = 1.15f;
    public float JumpHeight => jumpHeight;
    
    [Min(0), Tooltip("How much buffer time the player has for jumping.")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    public float JumpBufferTime => jumpBufferTime;
    
    [Min(0), Tooltip("Downward acceleration to keep the player going downwards. Higher = stronger downwards force.")]
    [SerializeField] private float gravity = 20;
    public float Gravity => gravity;
    
    
    
    [Header("Ground Check")]
    [Range(0.5f, 1), Tooltip("Ground check sphere size.")]
    [SerializeField] private float groundCheckRadiusScale = 0.9f;
    public float GroundCheckRadiusScale => groundCheckRadiusScale;
    
    [Min(0), Tooltip("How far below the feet still counts as grounded (m).")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    public float GroundCheckDistance => groundCheckDistance;
    
    [Min(0), Tooltip("How far above the capsule's bottom the cast starts (m).")]
    [SerializeField] private float groundCheckStartOffset = 0.05f;
    public float GroundCheckStartOffset => groundCheckStartOffset;
    
    [Range(0, 90), Tooltip("Steepest walkable surface (degrees).")]
    [SerializeField] private float maxSlopeAngle = 45;
    public float MaxSlopeAngle => maxSlopeAngle;
    
    [Min(0), Tooltip("Seconds after a jump during which ground is ignored.")]
    [SerializeField] private float groundIgnoreAfterJump = 0.1f;
    public float GroundIgnoreAfterJump => groundIgnoreAfterJump;
    
    [Tooltip("Layers that count as ground.")]
    [SerializeField] private LayerMask groundLayers;
    public LayerMask GroundLayers => groundLayers;
    
    [Header("Dash")]
    [Min(1), Tooltip("Number of dash charges.")]
    [SerializeField] private int maxDashCharges = 3;
    public int MaxDashCharges => maxDashCharges;
    
    [Min(0), Tooltip("Seconds to refill one charge.")]
    [SerializeField] private float dashRechargeTime = 1.5f;
    public float DashRechargeTime => dashRechargeTime;
    
    [Min(0), Tooltip("Speed the dash adds. (m/s)")]
    [SerializeField] private float dashSpeed = 10f;
    public float DashSpeed => dashSpeed;
    
    [Tooltip("Off: the dash adds to your velocity (a back dash brakes). On: all your speed turns to the dash direction, plus dash speed")]
    [SerializeField] private bool dashRedirect;
    public bool DashRedirect => dashRedirect;

    [Tooltip("Where the dash points")]
    [SerializeField] private DashDirectionMode dashDirectionMode = DashDirectionMode.CameraRelativeKeys;
    public DashDirectionMode DashDirectionMode => dashDirectionMode;
    
    [Tooltip("Backward dashes ignore camera pitch.")]
    [SerializeField] private bool flattenBackDash;
    public bool FlattenBackDash => flattenBackDash;
    
}
