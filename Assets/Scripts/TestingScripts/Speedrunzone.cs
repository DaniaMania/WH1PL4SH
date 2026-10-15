using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedrunZone : MonoBehaviour
{
    public enum ZoneType { Start, End }

    [SerializeField] private ZoneType zoneType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || !other.CompareTag("Player")) return;

        SpeedrunTrigger timer = SpeedrunTrigger.Instance;

        if (zoneType == ZoneType.Start) timer.StartRun();
        else timer.FinishRun();
    }
}