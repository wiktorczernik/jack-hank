using UnityEngine;
using UnityEngine.Events;

public class PickupableAmmo : SmashableEntity
{
    [Header("Ammo Crate")]
    public bool wasPickedUp = false;
    public UnityEvent onPickedUp;

    public int ammoInside;

    public void NotifyPickup()
    {
        if (!isAlive || wasPickedUp)
        {
            return;
        }
        wasPickedUp = true;
        hittable = false;
        OnPickupEvent();
        onPickedUp?.Invoke();
    }
    protected virtual void OnPickupEvent()
    {
        DisableColliders();
        //usedRigidbody.AddForce(Vector3.up * 5000, ForceMode.Force);
    }
    protected override void OnHitEvent()
    {
        base.OnHitEvent();
        Kill();
    }
}
