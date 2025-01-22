using UnityEngine;


public class PlayerVehicle : Vehicle
{
    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;


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
        OnPickupPassenger(trigger, passenger);

    }
    private void OnPickupPassenger(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        Destroy(passenger.gameObject);
        GameManager.runInfo.passengersOnBoard++;
    }
}
