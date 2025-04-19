using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SmashableEntity : GameEntity
{
    public int bountyPointsReward;
    public UnityEvent<SmashableEntity> OnHit;
    public bool wasHit = false;
    public bool hittable = true;
    public SmashableType SmashableType => smashableType;
    [Header("Components")]
    [SerializeField] protected CollisionEventEmitter collisionEvents;
    [SerializeField] protected Rigidbody[] usedRigidbodies;
    [SerializeField] protected Collider[] usedColliders;
    [SerializeField] private SmashableType smashableType;


    protected override void InternalExplode(ExplosionProperties explosionProps)
    {
        foreach (Rigidbody rb in usedRigidbodies)
        {
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(explosionProps.force, explosionProps.epicenterPosition, explosionProps.epicenterRadius);
        }
        OnHitEvent();
        OnHit?.Invoke(this);
        wasHit = true;
    }

    #region Event subscribing
    private void OnEnable()
    {
        collisionEvents.OnEnter?.AddListener(OnColliderHit);
    }
    private void OnDisable()
    {
        collisionEvents.OnEnter?.RemoveListener(OnColliderHit);
    }
    #endregion

    #region Events
    private void OnColliderHit(Collision collision)
    {
        if (collision.collider == null) return;

        SmashableEntity otherSmashable;
        bool isVehicleOrPass = collision.gameObject.tag == "Vehicle" || collision.gameObject.tag == "Passenger";
        bool isSmashable = collision.gameObject.TryGetComponent(out otherSmashable);

        if (!isVehicleOrPass && !isSmashable) return;
        
        if (!wasHit)
        {
            if (!hittable) return;
            if (isSmashable && !otherSmashable.wasHit) return;

            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 contactNormal = collision.contacts[0].normal;
            float relativeSpeed = collision.relativeVelocity.magnitude;
            foreach(Rigidbody rb in usedRigidbodies)
            {
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints.None;
            }
            OnHitEvent();
            OnHit?.Invoke(this);
            wasHit = true;
        }
    }

    protected virtual void OnHitEvent()
    {

    }

    public void SetRigidbodyKinematic(bool flag)
    {
        foreach (Rigidbody rb in usedRigidbodies)
        {
            rb.isKinematic = flag;
        }
    }
    public void EnableColliders()
    {
        foreach(Collider c in usedColliders)
        {
            c.enabled = true;
        }
    }
    public void DisableColliders()
    {
        foreach (Collider c in usedColliders)
        {
            c.enabled = false;
        }
    }
    #endregion
}

public enum SmashableType
{
    Cow, RoadSign, LightingPole, TrafficBarrier, TrafficLight, TrafficCone, OutdoorTable, Stall, Crate, Cart, Bench, Trash, Hydrant, SmallSign
}