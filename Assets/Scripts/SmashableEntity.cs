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
        if (collision.gameObject.tag != "Vehicle") return;
        if (!wasHit)
        {
            foreach(Rigidbody rb in usedRigidbodies)
            {
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints.None;
            }
            OnHit?.Invoke(this);
            wasHit = true;
        }
    }
    #endregion
}
