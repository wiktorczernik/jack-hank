using UnityEngine;

public class PickupZone : MonoBehaviour
{
    public GameObject pickupable;

    [SerializeField] TriggerEventEmitter eventEmitter;

    private void OnEnable()
    {
        eventEmitter.OnEnter.AddListener(OnEnter);
        eventEmitter.OnExit.AddListener(OnExit);
    }
    private void OnDisable()
    {
        eventEmitter.OnEnter.RemoveListener(OnEnter);
        eventEmitter.OnExit.RemoveListener(OnExit);
    }

    public void NotifyObsolete()
    {
        Destroy(gameObject);
    }

    public void OnEnter(Collider collider)
    {
        if (!collider) return;
        var player = collider.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        player.NotifyPickupZoneEnter(this);
    }
    public void OnExit(Collider collider)
    {
        if (!collider) return;
        var player = collider.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        player.NotifyPickupZoneExit();
    }
}
