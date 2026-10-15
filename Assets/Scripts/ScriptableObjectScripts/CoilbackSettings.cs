using UnityEngine;

[CreateAssetMenu(fileName = "CoilbackSettings", menuName = "Scriptable Objects/CoilbackSettings")]
public class CoilbackSettings : ScriptableObject
{
    [Header("Coilback")]
    [Min(0), Tooltip("Seconds to refill the Coilback meter.")]
    [SerializeField] private float coilbackRechargeTime = 10f;
    public float CoilbackRechargeTime => coilbackRechargeTime;
    
    [Min(0), Tooltip("Max amount of charge that the coilback can hold.")]
    [SerializeField] private float coilBackMaxCharge = 100f;
    public float CoilBackMaxCharge => coilBackMaxCharge;
    
    [Min(0), Tooltip("How much the Coilback charge decreases per fixed updated tick.")]
    [SerializeField] private float coilbackDecreaseAmount = 1f;
    public float CoilbackDecreaseAmount => coilbackDecreaseAmount;
}
