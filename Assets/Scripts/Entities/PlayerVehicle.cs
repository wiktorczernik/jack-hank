using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using JackHank.Cinematics;


public class PlayerVehicle : Vehicle
{
    [SerializeField] private Transform _seatsContainer;
    [SerializeField] private Transform _ammoTransitionSpot;

    public TriggerEventEmitter pickupRightTrigger;
    public TriggerEventEmitter pickupLeftTrigger;
    public CollisionEventEmitter vehicleCollision;

    public UnityEvent<TriggerEventEmitter, Pickupable> onPickupPassenger;
    public UnityEvent<ExplosionProperties> onExplosionNearby;
    public UnityEvent<TriggerEventEmitter, Pickupable> onPickupAmmo;
    public UnityEvent<PickupZone> onPickupZoneEnter;
    public UnityEvent<PickupZone> onPickupZoneExit;

    public PickupZone pickupZone;
    public PlayerTurret playerTurret;
    private Rigidbody _rigidbody;
    public PlayerVehicleSeatController seatsController;

    public BotVehicle _botDirect;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _botDirect = GetComponent<BotVehicle>();
    }

    private void OnEnable()
    {
        InitSeatsController();
        pickupRightTrigger.OnEnter.AddListener(col => OnTryPickup(pickupRightTrigger, col));
        pickupLeftTrigger.OnEnter.AddListener(col => OnTryPickup(pickupLeftTrigger, col));

        CinematicPlayer.onBeginPlay += OnCinematicBegin;
        CinematicPlayer.onEndPlay += OnCinematicEnd;
    }

    private void OnDisable()
    {
        CinematicPlayer.onBeginPlay -= OnCinematicBegin;
        CinematicPlayer.onEndPlay -= OnCinematicEnd;
    }


    public void NotifyPickupZoneEnter(PickupZone zone)
    {
        if (pickupZone == zone) return;
        pickupZone = zone;
        onPickupZoneEnter?.Invoke(zone);
        Debug.Log("Pickup zone enter");
    }
    public void NotifyPickupZoneExit()
    {
        if (pickupZone == null) return;
        onPickupZoneExit?.Invoke(pickupZone);
        pickupZone = null;
        Debug.Log("Pickup zone exit");
    }
    public void NotifyExplosionNearby(ExplosionProperties properties)
    {
        onExplosionNearby?.Invoke(properties);
    }

    public void OnThrottle(InputValue value)
    {
        physics.input.y = value.Get<float>();
    }
    public void OnBrake(InputValue value)
    {
        physics.input.y = -value.Get<float>();
    }
    public void OnSteer(InputValue value)
    {
        physics.input.x = value.Get<float>();
    }

    void OnTryPickup(TriggerEventEmitter triggerEmitter, Collider collider)
    {
        if (!collider) return;

        Pickupable pickupable;
        if (!collider.TryGetComponent(out pickupable)) return;
        if (!pickupable.IsPickupable()) return;

        pickupable.Pickup();

        switch(pickupable.type)
        {
            case PickupableType.Passenger:
                StartCoroutine(MovePassengerToSeat(pickupable)); break;
            case PickupableType.Ammo:
                StartCoroutine(MoveAmmoInside(pickupable)); break;
            default: break;
        }
    }

    private IEnumerator MovePassengerToSeat(Pickupable passenger)
    {
        var seat = seatsController.Occupy(passenger);
        if (seat == null) yield break;
        SmashableEntity smashable = passenger.parentEntity as SmashableEntity;
        smashable.SetRigidbodyKinematic(true);
        smashable.DisableColliders();
        
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

    private void InitSeatsController()
    {
        var count = _seatsContainer.childCount;
        var points = new Transform[count];
        for (var i = 0; i < count; i++) points[i] = _seatsContainer.GetChild(i);
        seatsController = new PlayerVehicleSeatController(points);
    }

    private IEnumerator MoveAmmoInside(Pickupable ammo)
    {
        var rb = ammo.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        var collider = ammo.GetComponent<BoxCollider>();
        collider.enabled = false;
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