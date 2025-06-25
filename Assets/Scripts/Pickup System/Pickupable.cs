using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public enum PickupableType
{
    None,
    Passenger,
    Ammo
}
public class Pickupable : MonoBehaviour
{
    [Header("State")]
    public bool pickedUp = false;
    public bool smashed = false;
    public bool expired = false;

    [Header("Options")]
    public GameEntity parentEntity;
    public int quantity = 1;
    public PickupableType type = PickupableType.None;
    public bool isSmashable = false;
    public bool canExpire = false;

    [Header("Events")]
    public UnityEvent onPickup;
    public UnityEvent onSmash;
    public UnityEvent onExpire;

    [Header("Audio events")]
    public EventReference pickupAudioRef;
    public EventReference smashAudioRef;
    public EventReference expireAudioRef;

    public bool IsPickupable()
    {
        return !(smashed || expired);
    }
    public void Pickup()
    {
        if (pickedUp || smashed || expired) return;

        RuntimeManager.PlayOneShotAttached(pickupAudioRef, gameObject);

        pickedUp = true;
        onPickup?.Invoke();

        if (type == PickupableType.Passenger) PassengerInteraction_GUI.PassengerCaught();
    }
    public void Expire()
    {
        if (pickedUp || smashed || expired || !canExpire) return;

        RuntimeManager.PlayOneShotAttached(expireAudioRef, gameObject);

        expired = false;
        onExpire?.Invoke();
        if (type == PickupableType.Passenger) PassengerInteraction_GUI.PassengerExpired();
    }
    public void Smash()
    {
        if (pickedUp || smashed || expired || !isSmashable) return;

        RuntimeManager.PlayOneShotAttached(smashAudioRef, gameObject);

        smashed = true;
        onSmash?.Invoke();
        if (type == PickupableType.Passenger) PassengerInteraction_GUI.PassengerSmashed();
    }
}