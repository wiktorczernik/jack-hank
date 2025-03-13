using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


public class PlayerVehicle : Vehicle
{
    public PlayerVehicleSeatController seatsController;
    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;
    public CollisionEventEmitter vehicleCollision;

    public UnityEvent<TriggerEventEmitter, PickupablePassenger> onPickupPassenger;

    [SerializeField] Transform _seatsContainer;


    public void OnMove(InputValue value)
    {
        physics.input = value.Get<Vector2>();
    }

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
        Vector3 initPos = passenger.transform.position;
        Vector3 initScale = passenger.transform.localScale;
        yield return null;
        float time = 0f;
        float maxTime = 0.2f;
        while (time < maxTime)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);
            passenger.transform.position = Vector3.Lerp(initPos, seat.point.position, time / maxTime);
            passenger.transform.localScale = Vector3.Lerp(initScale, initScale * 0.5f, time / maxTime);
            yield return null;
            
        }
        yield return null;
        passenger.transform.SetParent(seat.point);
        passenger.transform.position = seat.point.position;
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
