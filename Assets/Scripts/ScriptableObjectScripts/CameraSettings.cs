using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Scriptable Objects/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [Min(0), Tooltip("FOV at low speed.")]
    [SerializeField] private float baseFOV = 60;
    public float BaseFOV => baseFOV;
    
    [Min(0), Tooltip("Max FOV at high speed.")]
    [SerializeField] private float maxFOV = 120;
    public float MaxFOV => maxFOV;
    
    [Min(0), Tooltip("Speed that FOV starts widening.")]
    [SerializeField] private float fovStartSpeed = 10;
    public float FovStartSpeed => fovStartSpeed;
    
    [Min(0), Tooltip("Speed that FOV ends widening.")]
    [SerializeField] private float fovEndSpeed = 50;
    public float FovEndSpeed => fovEndSpeed;
    
    [Min(0), Tooltip("How fast FOV follows its target.")]
    [SerializeField] private float fovSmoothRate = 10;
    public float FovSmoothRate => fovSmoothRate;
    
    [Tooltip("How fast FOV follows its target.")]
    [SerializeField] private bool useTotalSpeed = true;
    public bool UseTotalSpeed => useTotalSpeed;
    
    [Min(0), Tooltip("Extra degrees added on a dash.")]
    [SerializeField] private float dashFovPunch = 10;
    public float DashFovPunch => dashFovPunch;
    
    [Min(0), Tooltip("How fast the punch fades.")]
    [SerializeField] private float dashPunchDecayRate = 10;
    public float DashPunchDecay => dashPunchDecayRate;
    
    [Min(0), Tooltip("Camera kick size.")]
    [SerializeField] private float dashImpulseStrength = 10;
    public float DashImpulseStrength => dashImpulseStrength;
    
    [Range(0, 1), Tooltip("0–1 accessibility multiplier.")]
    [SerializeField] private float effectsScale = 0;
    public float EffectsScale => effectsScale;
}
