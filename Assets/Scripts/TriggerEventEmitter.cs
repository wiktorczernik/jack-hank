using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEventEmitter : MonoBehaviour
{
    public UnityEvent<Collider> OnEnter;
    public UnityEvent<Collider> OnStay;
    public UnityEvent<Collider> OnExit;

    void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other);
    void OnTriggerStay(Collider other) => OnStay?.Invoke(other);
    void OnTriggerExit(Collider other) => OnExit?.Invoke(other);
}