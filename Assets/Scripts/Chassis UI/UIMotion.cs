using UnityEngine;
using UnityEngine.ProBuilder;

public class UIMotion : MonoBehaviour
{
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIMotionSettings uiMotionSettings;

    private Vector3 localPos;
    private float lastYaw;
    
    private bool wasGrounded;
    private float landingOffset;
    private float lastVerticalVelocity;
    private float fallTargetOffset;
    private float fallOffset;
    private float fallOffsetVelocity;

    void Awake()
    {
        localPos = transform.localPosition;
        lastYaw = transform.parent.eulerAngles.y;
        
        wasGrounded = playerController.IsGrounded;
        lastVerticalVelocity = playerBody.linearVelocity.y;
    }

    void FixedUpdate()
    {
        float verticalVelocity = playerBody.linearVelocity.y;
        float downwardAcceleration = (lastVerticalVelocity - verticalVelocity) / Time.fixedDeltaTime;
        lastVerticalVelocity = verticalVelocity;

        bool freefalling = !playerController.IsGrounded
            && verticalVelocity < -uiMotionSettings.StartFallSpeed;
        fallTargetOffset = freefalling && downwardAcceleration > 0f
            ? downwardAcceleration * uiMotionSettings.FallFlexPerAcceleration
            : 0f;
    }

    void LateUpdate()
    {
        bool grounded = playerController.IsGrounded;
        bool justLanded = !wasGrounded && grounded;
        wasGrounded = grounded;
        
        if (justLanded)
        {
            // Debug.Log("Landed");
            landingOffset = -uiMotionSettings.LandingDip;
        }
        
        landingOffset = Mathf.MoveTowards(landingOffset, 0f, 
            uiMotionSettings.LandingReturnSpeed * Time.deltaTime);
        
        float strength = Mathf.Clamp01(playerController.HorizontalSpeed / uiMotionSettings.FullBobSpeed);
        
        // bob is vertical (Y)
        float bob = Mathf.Sin(Time.time * uiMotionSettings.BobFrequency) * uiMotionSettings.MaxBobHeight * strength;
        
        float currentYaw = transform.parent.eulerAngles.y;
        float yawChange = Mathf.DeltaAngle(lastYaw, currentYaw);
        lastYaw = currentYaw;
        
        // sway is horizontal (X)
        float sway = Mathf.Clamp(yawChange * uiMotionSettings.SwayPerTurnDegree, -uiMotionSettings.MaxSway, 
            uiMotionSettings.MaxSway);
        
        float retreatStrength = Mathf.Clamp01(playerController.HorizontalSpeed / uiMotionSettings.FullRetreatSpeed);

        fallOffset = Mathf.SmoothDamp(fallOffset, grounded ? 0f : fallTargetOffset,
            ref fallOffsetVelocity, uiMotionSettings.FallFlexSettleTime);
        
        // retreat is depth (Z)
        float retreat = retreatStrength * uiMotionSettings.MaxRetreatDist;

        Vector3 x = Vector3.right * sway;
        Vector3 y = grounded ? Vector3.up * (bob + landingOffset) : Vector3.up * landingOffset;
        Vector3 z = Vector3.forward * (retreat + fallOffset);
        
        transform.localPosition = localPos + x + y + z;
    }
}
