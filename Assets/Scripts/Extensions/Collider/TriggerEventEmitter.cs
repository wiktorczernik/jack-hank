using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider)), System.Serializable]
public class TriggerEventEmitter : BaseColliderEventEmitter<Collider>
{
    void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other);
    void OnTriggerStay(Collider other) => OnStay?.Invoke(other);
    void OnTriggerExit(Collider other) => OnExit?.Invoke(other);
}