using UnityEngine;
using UnityEngine.UI;

public class DashMeterUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image[] dashCharges;
    
    private float _fullOpacity = 1f;
    private float _rechargingOpacity = 0.5f;    

    void Update()
    {
        for(int i = 0; i < dashCharges.Length; i++)
        {
            float fillAmount = Mathf.Clamp01(playerController.DashCharge - i);
            
            Color currentColor = dashCharges[i].color;

            if (fillAmount >= 1)
            {
                currentColor.a = _fullOpacity;
            }
            else
            {
                currentColor.a = _rechargingOpacity;
            }
            dashCharges[i].fillAmount = fillAmount;
            dashCharges[i].color = currentColor;
            
        }
    }
}
