using UnityEngine;
using UnityEngine.Events;

public class SmashableEntity : MonoBehaviour
{
    public UnityEvent OnHit;
    [Header("Components")]
    [SerializeField] CollisionEventEmitter collisionEvents;
    [SerializeField] Rigidbody usedRigidbody;


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
        usedRigidbody.freezeRotation = false;
        usedRigidbody.constraints = RigidbodyConstraints.None;
        OnHit?.Invoke();
    }
    #endregion
}
