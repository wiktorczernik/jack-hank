using UnityEngine;
using UnityEngine.Events;

public class SmashableEntity : MonoBehaviour
{
    public int bountyPointsReward;
    public UnityEvent<SmashableEntity> OnHit;
    public bool wasHit = false;
    [Header("Components")]
    [SerializeField] CollisionEventEmitter collisionEvents;
    [SerializeField] Rigidbody[] usedRigidbodies;


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
        if (collision.gameObject.tag != "Vehicle" && collision.gameObject.tag != "Passenger") return;
        if (!wasHit)
        {
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
