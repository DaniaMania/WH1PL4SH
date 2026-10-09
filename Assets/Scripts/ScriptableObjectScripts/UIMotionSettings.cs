using UnityEngine;

[CreateAssetMenu(fileName = "UIMotionSettings", menuName = "Scriptable Objects/UIMotionSettings")]
public class UIMotionSettings : ScriptableObject
{
    [SerializeField] private float maxBobHeight = 0.01f;
    [SerializeField] private float fullBobSpeed = 30f;
    [SerializeField] private float bobFrequency = 10f;
    
    [SerializeField] private float swayPerTurnDegree = 0.002f;
    [SerializeField] private float maxSway = 0.03f;
    
    [SerializeField] private float fullRetreatSpeed = 30f;
    [SerializeField] private float maxRetreatDist = 0.25f;

    [SerializeField] private float landingDip = 0.04f;
    [SerializeField] private float landingReturnSpeed = 0.3f;
    
    [SerializeField] private float startFallSpeed = 1f;
    [SerializeField] private float fallFlexPerAcceleration = 0.001f;
    [SerializeField] private float fallFlexSettleTime = 0.15f;
    
    public float MaxBobHeight => maxBobHeight;
    public float FullBobSpeed => fullBobSpeed;
    public float BobFrequency => bobFrequency;
    public float SwayPerTurnDegree => swayPerTurnDegree;
    public float MaxSway => maxSway;
    public float FullRetreatSpeed => fullRetreatSpeed;
    public float MaxRetreatDist => maxRetreatDist;
    public float LandingDip => landingDip;
    public float LandingReturnSpeed => landingReturnSpeed;
    public float StartFallSpeed => startFallSpeed;
    public float FallFlexPerAcceleration => fallFlexPerAcceleration;
    public float FallFlexSettleTime => fallFlexSettleTime;
}
