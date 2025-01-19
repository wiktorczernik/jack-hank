using UnityEngine;
using UnityEngine.Events;


public sealed class CollisionEventEmitter : MonoBehaviour
{
    public UnityEvent<Collision> OnEnter;
    public UnityEvent<Collision> OnStay;
    public UnityEvent<Collision> OnExit;

    void OnCollisionEnter(Collision collision) => OnEnter?.Invoke(collision);
    void OnCollisionStay(Collision collision) => OnStay?.Invoke(collision);
    void OnCollisionExit(Collision collision) => OnExit?.Invoke(collision);
}