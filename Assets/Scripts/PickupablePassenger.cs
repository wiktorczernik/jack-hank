using UnityEngine;
using UnityEngine.Events;

public class PickupablePassenger : SmashableEntity
{
    public bool isAlive = true;
    public bool isPickedUp = false;

    public UnityEvent onPickedUp;

    public void NotifyPickup()
    {
        isPickedUp = true;
        OnPickupEvent();
        onPickedUp?.Invoke();
    }
    protected virtual void OnPickupEvent()
    {
        foreach(Rigidbody rb in usedRigidbodies)
        {
            rb.AddForce(Vector3.up * 50, ForceMode.Impulse);
        }
        Invoke(nameof(Disappear), 0.5f);
    }
    private void Disappear()
    {
        Destroy(gameObject);
    }
    protected override void OnHitEvent()
    {
        base.OnHitEvent();
        isAlive = false;
    }
}
