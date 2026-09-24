using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
public class MovementSettings : ScriptableObject
{
    [Header("Ground Movement")] 
    [Min(0), Tooltip("Speed your input aims for on the ground. Actual speed can exceed this with other movement methods. Higher = faster running cap on the ground.")]
    [SerializeField] private float groundWishSpeed = 8;
    public float GroundWishSpeed => groundWishSpeed; 
    
    /* how to use: int currentGroundWishSpeed = myScriptableObject.GroundWishSpeed; */
    
    [Min(0), Tooltip("How quickly you reach ground wish speed. Higher = faster build up.")]
    [SerializeField] private float groundAcceleration = 10;
    public float GroundAcceleration => groundAcceleration;
    
    [Min(0), Tooltip("How fast ground speed decreases. Higher = less slide.")]
    [SerializeField] private float friction = 4;
    public float Friction => friction;
    
    [Min(0), Tooltip("Below this speed stopSpeed replaces friction for a sudden stop. Higher = higher value for final stop.")]
    [SerializeField] private float stopSpeed = 2.5f;
    public float StopSpeed => stopSpeed;
    
    [Header("Air Movement")] 
    [Range(0,10), Tooltip("Caps wish speed while airborne. Doesn't limit air speed.")]
    [SerializeField] private float airWishSpeedCap = 0.75f;
    public float AirWishSpeedCap => airWishSpeedCap;
    
    [Min(0), Tooltip("How strongly you can steer in the air.")]
    [SerializeField] private float airAcceleration = 10;
    public float AirAcceleration => airAcceleration;
    
    [Header("Jump and Gravity")]
    [Min(0), Tooltip("How high you can jump. Jump is calculated from this and gravity.")]
    [SerializeField] private float jumpHeight = 1.15f;
    public float JumpHeight => jumpHeight;
    
    [Min(0), Tooltip("Downward acceleration to keep the player grounded. Higher = stronger downwards force.")]
    [SerializeField] private float gravity = 20;
    public float Gravity => gravity;
    
    
    // no ground check yet
}
