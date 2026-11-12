using UnityEngine;
using Unity.Cinemachine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private CameraSettings cameraSettings;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Camera playerUICamera;

    private float _currentFOV;

    void Start()
    {
        _currentFOV = cameraSettings.BaseFOV;
    }

    void Update()
    {
        float speed = (cameraSettings.UseTotalSpeed ? playerController.TotalSpeed : playerController.HorizontalSpeed);
        float t = Mathf.InverseLerp(cameraSettings.FovStartSpeed, cameraSettings.FovEndSpeed, speed);
        float target = Mathf.Lerp(cameraSettings.BaseFOV, cameraSettings.BaseFOV + (cameraSettings.MaxFOV - cameraSettings.BaseFOV) * cameraSettings.EffectsScale, t);
        _currentFOV = Mathf.Lerp(_currentFOV, target, 1 - Mathf.Exp(-cameraSettings.FovSmoothRate * Time.deltaTime));
        playerCamera.Lens.FieldOfView = _currentFOV;
        playerUICamera.fieldOfView = _currentFOV;
    }
}
