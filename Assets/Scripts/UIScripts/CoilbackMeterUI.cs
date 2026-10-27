using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoilbackMeterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image coilbackMeter;
    [SerializeField] private TextMeshProUGUI coilbackMeterText;
    [SerializeField] private TextMeshProUGUI coilbackCooldownText;
    
    [Header("Coilback Rewind")]
    [SerializeField] private CoilbackRewind coilbackRewind;
    
    [Header("Coilback Settings")]
    [SerializeField] private CoilbackSettings coilback;
    
    private float _usableOpacity = 1f;
    private float _unUsableOpacity = 0.5f;
    private Color _storedColor;

    void Start()
    {
        // Store the color of the Coilback meter immediately
        _storedColor = coilbackMeter.color;
    }
    
    void Update()
    {
        // How much the bar is filled (through fill amount) is determined by the Coilback current charge clamped to between 0 and 1
        float fillAmount = Mathf.Clamp01(coilbackRewind.CoilBackCharge/coilback.CoilBackMaxCharge);

        // Truncate the numeric Coilback charge so that it displays as a whole number
        coilbackMeterText.text = ((int)coilbackRewind.CoilBackCharge).ToString();
            
        Color currentColor = coilbackMeter.color;
        
        // Opacity of the Coilback bar being lower if under 25% (unusable under 25%)
        if (!coilbackRewind.RewindOnCooldown && fillAmount >= 0.25f)
        {
            currentColor = _storedColor;
            currentColor.a = _usableOpacity;
            
            coilbackCooldownText.color = currentColor;
            
            if (coilbackRewind.IsRewinding)
            {
                coilbackCooldownText.text = "is rewinding";
            }
            else
            {
                coilbackCooldownText.text = "can use";
            }
        }
        else
        {
            currentColor = Color.lightCoral;
            currentColor.a = _unUsableOpacity;

            double truncatedCooddown = Math.Truncate(coilbackRewind.RewindCoolDownTime * 100) / 100;
            
            coilbackCooldownText.text = "on cooldown (" + truncatedCooddown.ToString("F2")  + ")";
            
            coilbackCooldownText.color = Color.red;
        }
        
        coilbackMeter.fillAmount = fillAmount;
        coilbackMeter.DOColor(currentColor, .5f);
    }
}
