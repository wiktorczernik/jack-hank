using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerVehicle : Vehicle
{
    [SerializeField] private Transform _seatsContainer;
    [SerializeField] private Transform _ammoTransitionSpot;

    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;
    public CollisionEventEmitter vehicleCollision;

    public UnityEvent<TriggerEventEmitter, PickupablePassenger> onPickupPassenger;
    public UnityEvent<ExplosionProperties> onExplosionNearby;
    public UnityEvent<TriggerEventEmitter, PickupableAmmo> onPickupAmmo;

    public PlayerTurret playerTurret;
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

        pickupRightTrigger.OnEnter.AddListener(col => OnTryPickupAmmo(pickupRightTrigger, col));
        pickupLeftTrigger.OnEnter.AddListener(col => OnTryPickupAmmo(pickupLeftTrigger, col));

        CinematicPlayer.onBeginPlay += OnCinematicBegin;
        CinematicPlayer.onEndPlay += OnCinematicEnd;
    }

    private void OnDisable()
    {
        CinematicPlayer.onBeginPlay -= OnCinematicBegin;
        CinematicPlayer.onEndPlay -= OnCinematicEnd;
    }


    public void NotifyExplosionNearby(ExplosionProperties properties)
    {
        onExplosionNearby?.Invoke(properties);
    }

    public void OnMove(InputValue value)
    {
        physics.input = value.Get<Vector2>();
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        var rotationCorrection = Quaternion.Euler(0, 90, 0);
        _rigidbody.position = position;
        _rigidbody.rotation = rotation * rotationCorrection;
        physics.TeleportWheels(position);
    }

    public void Teleport(Vector3 position)
    {
        _rigidbody.position = position;
        physics.TeleportWheels(position);
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

    private void OnTryPickupAmmo(TriggerEventEmitter trigger, Collider col)
    {
        if (!col) return;
        if (col.tag != "Ammo") return;
        if (!col.TryGetComponent(out PickupableAmmo ammo)) return;
        if (!ammo.isAlive || ammo.wasPickedUp) return;
        playerTurret.LoadAmmo(ammo.ammoInside);
        OnPickupAmmoEvent(trigger, ammo);
        onPickupAmmo?.Invoke(trigger, ammo);
        StartCoroutine(MoveAmmoInside(ammo));
    }

    private IEnumerator MoveAmmoInside(PickupableAmmo ammo)
    {
        ammo.SetRigidbodyKinematic(true);
        ammo.DisableColliders();
        var initPos = ammo.transform.position;
        var initScale = ammo.transform.localScale;
        yield return null;
        var time = 0f;
        var maxTime = 0.2f;
        while (time < maxTime)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);
            ammo.transform.position = Vector3.Lerp(initPos, _ammoTransitionSpot.position, time / maxTime);
            ammo.transform.localScale = Vector3.Lerp(initScale, initScale * 0.5f, time / maxTime);
            yield return null;
        }

        yield return null;
        Destroy(ammo.gameObject);
    }

    private void OnPickupAmmoEvent(TriggerEventEmitter trigger, PickupableAmmo ammo)
    {
        ammo.NotifyPickup();
    }

    private void OnCinematicBegin()
    {
        _rigidbody.isKinematic = true;
        physics.enabled = false;
    }

    private void OnCinematicEnd()
    {
        _rigidbody.isKinematic = false;
        physics.enabled = true;
    }

    protected override void OnDeathInternal()
    {
        GameManager.PlayerDeathRestart();
    }
}