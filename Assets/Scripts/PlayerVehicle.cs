using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerVehicle : Vehicle
{
    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;
    public CollisionEventEmitter vehicleCollision;

    public UnityEvent<TriggerEventEmitter, PickupablePassenger> onPickupPassenger;
    public UnityEvent<ExplosionProperties> onExplosionNearby;

    [SerializeField] private Transform _seatsContainer;

    private Rigidbody _rigidbody;
    public PlayerVehicleSeatController seatsController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        InitSeatsController();
        pickupRightTrigger.OnEnter.AddListener(col => OnTryPickupPassenger(pickupRightTrigger, col));
        pickupLeftTrigger.OnEnter.AddListener(col => OnTryPickupPassenger(pickupLeftTrigger, col));
    }


    public void NotifyExplosionNearby(ExplosionProperties properties)
    {
        onExplosionNearby?.Invoke(properties);
    }

    public void OnMove(InputValue value)
    {
        physics.input = value.Get<Vector2>();
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        _rigidbody.position = position;
        _rigidbody.rotation = rotation;
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
        var seat = seatsController.Occupy(passenger);
        if (seat == null) return;
        StartCoroutine(MovePassengerToSeat(seat));
    }

    private IEnumerator MovePassengerToSeat(PlayerVehicleSeat seat)
    {
        var passenger = seat.passenger;
        passenger.SetRigidbodyKinematic(true);
        passenger.DisableColliders();
        var initPos = passenger.transform.position;
        var initScale = passenger.transform.localScale;
        yield return null;
        var time = 0f;
        var maxTime = 0.2f;
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
        var count = _seatsContainer.childCount;
        var points = new Transform[count];
        for (var i = 0; i < count; i++) points[i] = _seatsContainer.GetChild(i);
        seatsController = new PlayerVehicleSeatController(points);
    }
}