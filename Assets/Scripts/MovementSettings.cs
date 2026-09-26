using UnityEngine;

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
    
    [Header("Air Movement")] 
    [Range(0,10), Tooltip("Caps wish speed while airborne. Doesn't limit air speed.")]
    [SerializeField] private float airWishSpeedCap = 0.75f;
    public float AirWishSpeedCap => airWishSpeedCap;
    
    [Range(0, 10), Tooltip("How strongly you steer in the air. Higher = easier speed gain and tighter air turns.")]
    [SerializeField] private float airAcceleration = 4;
    public float AirAcceleration => airAcceleration;
    
    [Header("Jump and Gravity")]
    [Min(0), Tooltip("How high you can jump. Jump is calculated from this and gravity.")]
    [SerializeField] private float jumpHeight = 1.15f;
    public float JumpHeight => jumpHeight;
    
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
}
