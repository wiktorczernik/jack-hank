using UnityEngine;
using UnityEngine.Events;


public class PlayerVehicle : Vehicle
{
    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;

    public UnityEvent<TriggerEventEmitter, PickupablePassenger> onPickupPassenger;


    private void OnEnable()
    {
        pickupRightTrigger.OnEnter.AddListener((Collider col) => OnTryPickupPassenger(pickupRightTrigger, col));
        pickupLeftTrigger.OnEnter.AddListener((Collider col ) => OnTryPickupPassenger(pickupLeftTrigger, col));
    }

    private void OnTryPickupPassenger(TriggerEventEmitter trigger, Collider col)
    {
        if (!col) return;
        if (col.tag != "Passenger") return;
        PickupablePassenger passenger;
        if (!col.TryGetComponent(out passenger)) return;
        if (!passenger.isAlive) return;
        OnPickupPassengerEvent(trigger, passenger);
        onPickupPassenger?.Invoke(trigger, passenger);

    }
    private void OnPickupPassengerEvent(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        passenger.NotifyPickup();
    }
}
