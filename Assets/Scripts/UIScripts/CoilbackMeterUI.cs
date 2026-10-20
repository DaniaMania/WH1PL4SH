using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoilbackMeterUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image coilbackMeter;
    [SerializeField] private TextMeshProUGUI coilbackMeterText;
    
    [Header("Coilback Rewind")]
    [SerializeField] private CoilbackRewind coilbackRewind;
    
    [Header("Coilback Settings")]
    [SerializeField] private CoilbackSettings coilback;
    
    private float _usableOpacity = 1f;
    private float _unUsableOpacity = 0.3f;
    private Color _storedColor;

    void Start()
    {
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
        if (fillAmount >= 0.25f)
        {
            currentColor.a = _usableOpacity;
            currentColor = _storedColor;
        }
        else
        {
            currentColor = Color.red;
            currentColor.a = _unUsableOpacity;
        }
        
        coilbackMeter.fillAmount = fillAmount;
        coilbackMeter.color = currentColor;
    }
}
