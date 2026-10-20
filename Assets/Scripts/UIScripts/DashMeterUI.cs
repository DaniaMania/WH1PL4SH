using UnityEngine;
using UnityEngine.UI;

public class DashMeterUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image[] dashCharges;
    
    private float _fullOpacity = 1f;
    private float _rechargingOpacity = 0.5f;
    private Color _storedColor;

    void Start()
    {
        // Store the color of the dash charge immediately
        _storedColor = dashCharges[0].color;
    }
    void Update()
    {
        // For loop that goes over each dash charge
        for(int i = 0; i < dashCharges.Length; i++)
        {
            // Fill amount is determined by the playerController's _dashCharge float
            float fillAmount = Mathf.Clamp01(playerController.DashCharge - i);
            
            // Local variable to store the color of the current dash bar's charge
            Color currentColor = dashCharges[i].color;

            if (fillAmount >= 1)
            {
                // If the fill amount is 1 or greater the bar is the storedColor and full opacity
                currentColor = _storedColor;
                currentColor.a = _fullOpacity;
            }
            else
            {
                // If less than 1 then it is red and recharging opacity
                currentColor = Color.lightCoral;
                currentColor.a = _rechargingOpacity;
            }
            dashCharges[i].fillAmount = fillAmount;
            dashCharges[i].color = currentColor;
        }
    }
}
