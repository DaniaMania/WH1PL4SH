using UnityEngine;
using Unity.Cinemachine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private CameraSettings cameraSettings;
    [SerializeField] private PlayerController playerController;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CinemachineCamera playerCamera;

    private float _currentFOV;
    private float _dashPunch;

    void Start()
    {
        _currentFOV = cameraSettings.BaseFOV;
    }

    void Update()
    {
        
    }
    
    void HandleDashed(Vector3 dashDirection)
    {
        impulseSource.GenerateImpulseWithVelocity(dashDirection * cameraSettings.DashImpulseStrength * cameraSettings.EffectsScale);
        _dashPunch = cameraSettings.DashFovPunch * cameraSettings.EffectsScale;
    }
    
    void OnEnable()  { playerController.Dashed += HandleDashed; }
    void OnDisable() { playerController.Dashed -= HandleDashed; }
}
