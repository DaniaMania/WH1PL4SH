using UnityEngine;
using UnityEngine.ProBuilder;

public class UIMotion : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIMotionSettings uiMotionSettings;

    private Vector3 localPos;
    private float lastYaw;

    void Awake()
    {
        localPos = transform.localPosition;
        lastYaw = transform.parent.eulerAngles.y;
    }

    void LateUpdate()
    {
        float strength = Mathf.Clamp01(playerController.HorizontalSpeed / uiMotionSettings.FullBobSpeed);
        
        // bob is vertical (Y)
        float bob = Mathf.Sin(Time.time * uiMotionSettings.BobFrequency) * uiMotionSettings.MaxBobHeight * strength;
        
        float currentYaw = transform.parent.eulerAngles.y;
        float yawChange = Mathf.DeltaAngle(lastYaw, currentYaw);
        lastYaw = currentYaw;
        
        // sway is horizontal (X)
        float sway = Mathf.Clamp(yawChange * uiMotionSettings.SwayPerTurnDegree, -uiMotionSettings.MaxSway, uiMotionSettings.MaxSway);
        
        float retreatStrength = Mathf.Clamp01(playerController.HorizontalSpeed / uiMotionSettings.FullRetreatSpeed);
        
        // retreat is depth (Z)
        float retreat = retreatStrength * uiMotionSettings.MaxRetreatDist;
        
        transform.localPosition = localPos + Vector3.up * bob + Vector3.right * sway + Vector3.forward * retreat;
    }
}
