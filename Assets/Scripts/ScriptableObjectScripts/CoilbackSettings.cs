using UnityEngine;

[CreateAssetMenu(fileName = "CoilbackSettings", menuName = "Scriptable Objects/CoilbackSettings")]
public class CoilbackSettings : ScriptableObject
{
    [Header("Coilback")]
    [Min(0), Tooltip("Time it takes to fully refill the Coilback meter from min to max. (s)")]
    [SerializeField] private float coilbackRechargeTime = 10f;
    public float CoilbackRechargeTime => coilbackRechargeTime;
    
    [Min(0), Tooltip("Max amount of charge that the coilback can hold.")]
    [SerializeField] private float coilBackMaxCharge = 100f;
    public float CoilBackMaxCharge => coilBackMaxCharge;
    
    [Min(0), Tooltip("How much the Coilback charge decreases per fixed updated tick.")]
    [SerializeField] private float coilbackDecreaseAmount = 1f;
    public float CoilbackDecreaseAmount => coilbackDecreaseAmount;
    
    [Min(0), Tooltip("How long the cooldown for the Coilback is. (s)")]
    [SerializeField] private float coilbackCooldownDuration = 3f;
    public float CoilbackCooldownDuration => coilbackCooldownDuration;
    
    [Range(1, 20), Tooltip("Speed scale for the Coilback. (Lower = higher multiplier. Default 10)")]
    [SerializeField] private int coilbackSpeedScale = 10;
    public float CoilbackSpeedScale => coilbackSpeedScale;
}
