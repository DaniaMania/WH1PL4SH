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
    
    void Update()
    {
        float fillAmount = Mathf.Clamp01(coilbackRewind.CoilBackCharge/coilback.CoilBackMaxCharge);

        coilbackMeterText.text = ((int)coilbackRewind.CoilBackCharge).ToString();
            
        Color currentColor = coilbackMeter.color;

        if (fillAmount >= 0.25f)
        {
            currentColor.a = _usableOpacity;
        }
        else
        {
            currentColor.a = _unUsableOpacity;
        }
        coilbackMeter.fillAmount = fillAmount;
        coilbackMeter.color = currentColor;
    }
}
