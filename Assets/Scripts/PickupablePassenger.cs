using UnityEngine;
using UnityEngine.Events;

public class PickupablePassenger : SmashableEntity
{
    public int bountyPointsPenalty = 5000;
    public bool isAlive = true;
    public bool isLookingForPlayerVehicle = false;
    public bool isPickedUp = false;
    public PlayerVehicle playerVehicle;

    public UnityEvent onPickedUp;

    public void NotifyPickup()
    {
        isPickedUp = true;
        hittable = false;
        OnPickupEvent();
        onPickedUp?.Invoke();
    }
    protected virtual void OnPickupEvent()
    {
        foreach(Rigidbody rb in usedRigidbodies)
        {
            rb.AddForce(Vector3.up * 50, ForceMode.Impulse);
        }
        StopLookingForPlayerVehicle();
    }
    public void StartLookingForPlayerVehicle(PlayerVehicle vehicle)
    {
        if (isLookingForPlayerVehicle) return;
        isLookingForPlayerVehicle = true;
        playerVehicle = vehicle;
    }
    public void StopLookingForPlayerVehicle()
    {
        if (!isLookingForPlayerVehicle) return;
        isLookingForPlayerVehicle = false;
        playerVehicle = null;
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

    private void LateUpdate()
    {
        if (isLookingForPlayerVehicle)
        {
            Vector3 pos = transform.position;
            Vector3 targetPos = playerVehicle.transform.position;
            transform.LookAt(new Vector3(targetPos.x, pos.y, targetPos.z));
        }
    }
}
