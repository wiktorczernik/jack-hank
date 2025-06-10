using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent onEnter;
    public bool destroyOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other) return;
        PlayerVehicle vehicle = other.GetComponentInParent<PlayerVehicle>();
        if (!vehicle) return;
        onEnter?.Invoke();
        if (destroyOnTrigger)
            Destroy(gameObject);
    }
}
