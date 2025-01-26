using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SmashableEntity : MonoBehaviour
{
    public int bountyPointsReward;
    public UnityEvent<SmashableEntity> OnHit;
    public bool wasHit = false;
    public bool hittable = true;
    [Header("Components")]
    [SerializeField] protected CollisionEventEmitter collisionEvents;
    [SerializeField] protected Rigidbody[] usedRigidbodies;


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
    #endregion
}
