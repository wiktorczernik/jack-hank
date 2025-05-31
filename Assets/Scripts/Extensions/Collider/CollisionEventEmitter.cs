using UnityEngine;
using UnityEngine.Events;


public sealed class CollisionEventEmitter : BaseColliderEventEmitter<Collision>
{
    void OnCollisionEnter(Collision collision) => OnEnter?.Invoke(collision);
    void OnCollisionStay(Collision collision) => OnStay?.Invoke(collision);
    void OnCollisionExit(Collision collision) => OnExit?.Invoke(collision);
}