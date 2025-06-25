using System;
using UnityEngine;

public class PickupZone : MonoBehaviour
{
    public Pickupable target;
    public float minDelayBeforeExpireInSecons;

    [SerializeField] TriggerEventEmitter eventEmitter;

    private float enterTime;


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

    public void OnEnter(Collider collider)
    {
        if (!collider) return;
        var player = collider.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        player.NotifyPickupZoneEnter(this);
        enterTime = Time.time;
    }
    public void OnExit(Collider collider)
    {
        if (!collider) return;
        var player = collider.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        player.NotifyPickupZoneExit();

        if (Time.time - enterTime < minDelayBeforeExpireInSecons) return;

        target.Expire();
    }
}
