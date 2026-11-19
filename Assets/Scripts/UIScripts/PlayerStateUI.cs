using System;
using UnityEngine;
using TMPro;

public class PlayerStateUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI playerStateText;
    [SerializeField] private TextMeshProUGUI speedtext;
    
    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerRb;

    void Update()
    {
        Vector3 velocity = playerRb.linearVelocity;

        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);
        
        double trucatedHorizontalVelocity = Math.Truncate(horizontalVelocity.magnitude * 10)/10;
        
        double trucatedVelocity = Math.Truncate(velocity.magnitude * 10)/10;
        
        speedtext.text = trucatedHorizontalVelocity.ToString("F1") + " m/s";
    }
}
