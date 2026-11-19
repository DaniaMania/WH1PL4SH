using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Header("Walking")] 
    [Min(0), Tooltip("Speed that the player aims for (max speed) when in walking mode and grounded.")]
    [SerializeField] private float walkGroundSpeed = 10;
    public float WalkGroundSpeed => walkGroundSpeed; 
    
    [Min(0), Tooltip("Player acceleration when in walking mode and grounded.")]
    [SerializeField] private float walkGroundAcceleration = 5;
    public float WalkGroundAcceleration => walkGroundAcceleration;
    
    [Min(0), Tooltip("Player acceleration when in walking mode and not grounded.")]
    [SerializeField] private float walkAirAcceleration = 2;
    public float WalkAirAcceleration => walkAirAcceleration;
    
    [Min(0), Tooltip("Player friction when in walking mode.")]
    [SerializeField] private float walkFriction = 7;
    public float WalkFriction => walkFriction;
    
    
    [Header("Running")] 
    
    
    
    
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
}