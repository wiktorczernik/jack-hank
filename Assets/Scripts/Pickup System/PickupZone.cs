using System;
using UnityEngine;
using UnityEngine.Events;

public class PickupZone : MonoBehaviour
{
    public Pickupable target;

    [SerializeField] TriggerEventEmitter eventEmitter;
    public float minDelayBeforeExire;

    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    [Header("State")]
    public bool isZonePassed;
    public float enterZoneTime;


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
        onPlayerEnter.Invoke();
        player.NotifyPickupZoneEnter(this);

        enterZoneTime = Time.time;

    }
    public void OnExit(Collider collider)
    {
        if (!collider) return;
        var player = collider.GetComponentInParent<PlayerVehicle>();
        if (!player) return;
        onPlayerExit.Invoke();
        player.NotifyPickupZoneExit();

        if (isZonePassed || Time.time - enterZoneTime < minDelayBeforeExire) return;

        target.Expire();
        isZonePassed = true;    
    }
}
