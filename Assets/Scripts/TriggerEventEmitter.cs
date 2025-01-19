using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEventEmitter : MonoBehaviour
{
    public readonly UnityEvent<Collider> OnEnter;
    public readonly UnityEvent<Collider> OnStay;
    public readonly UnityEvent<Collider> OnExit;

    void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other);
    void OnTriggerStay(Collider other) => OnStay?.Invoke(other);
    void OnTriggerExit(Collider other) => OnExit?.Invoke(other);
}