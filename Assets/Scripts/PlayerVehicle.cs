using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class PlayerVehicle : Vehicle
{
    public PlayerVehicleSeatController seatsController;
    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;

    public UnityEvent<TriggerEventEmitter, PickupablePassenger> onPickupPassenger;

    [SerializeField] Transform _seatsContainer;


    private void OnEnable()
    {
        InitSeatsController();
        pickupRightTrigger.OnEnter.AddListener((Collider col) => OnTryPickupPassenger(pickupRightTrigger, col));
        pickupLeftTrigger.OnEnter.AddListener((Collider col ) => OnTryPickupPassenger(pickupLeftTrigger, col));
    }

    private void OnTryPickupPassenger(TriggerEventEmitter trigger, Collider col)
    {
        if (!col) return;
        if (col.tag != "Passenger") return;
        PickupablePassenger passenger;
        if (!col.TryGetComponent(out passenger)) return;
        if (!passenger.isAlive || passenger.wasPickedUp) return;
        OnPickupPassengerEvent(trigger, passenger);
        onPickupPassenger?.Invoke(trigger, passenger);
        PlayerVehicleSeat seat = seatsController.Occupy(passenger);
        if (seat == null) return;
        StartCoroutine(MovePassengerToSeat(seat));

    }
    IEnumerator MovePassengerToSeat(PlayerVehicleSeat seat)
    {
        PickupablePassenger passenger = seat.passenger;
        passenger.SetRigidbodyKinematic(true);
        passenger.DisableColliders();
        yield return null;
        passenger.transform.position = seat.point.position;
        passenger.transform.localScale *= 0.5f;
        seat.passenger.transform.SetParent(seat.point);
        yield return null;
    }
    private void OnPickupPassengerEvent(TriggerEventEmitter trigger, PickupablePassenger passenger)
    {
        passenger.NotifyPickup();
    }

    private void InitSeatsController()
    {
        int count = _seatsContainer.childCount;
        Transform[] points = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            points[i] = _seatsContainer.GetChild(i);
        }
        seatsController = new PlayerVehicleSeatController(points);
    }
}
